using Autofac;
using FluentValidation.AspNetCore;
using LastContent.Middleware.Logging;
using LastContent.ServiceBus.Core;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Email;
using LastContent.Utilities.Notification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Subby.Blazor.Maui.Controllers;
using Subby.Blazor.Maui.Data;
using Subby.Blazor.Maui.Extensions;
using Subby.Core.Entities;
using Subby.Core.Interfaces;
using Subby.Core.Mappings;
using Subby.Core.Services;
using Subby.Infrastructure;
using Subby.Infrastructure.Data;
using Subby.Infrastructure.Factory;
using Subby.Infrastructure.Middlewares;
using Subby.Infrastructure.UserStores;
using Subby.Utilities.Authentication;
using Subby.Utilities.Interfaces;
using Subby.Utilities.Middleware;
using System.Reflection;
using System.Text;


namespace Subby.Blazor.Maui
{
    public static class MauiProgram
    {
        private static IConfiguration configuration;
        //private static readonly IWebHostEnvironment _env;

        public static IConfiguration Configuration { get => configuration; set => configuration = value; }

        public static MauiApp CreateMauiApp()
        {
            Configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            var builder = MauiApp.CreateBuilder();
            IServiceCollection services = builder.Services; 

            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddBlazorWebView();
            builder.Services.AddSingleton<WeatherForecastService>();


            services.AddNHibernate(
                Configuration.GetConnectionString("SQLConnection"),
                Assembly.GetAssembly(typeof(Session)),
                Assembly.GetAssembly(typeof(SessionMappingOverride)));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisCache");
            });

            services.AddHealthChecks();
            services.AddControllersWithViews().AddNewtonsoftJson();

            var viewBuilder = services.AddControllersWithViews(options => { })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                ).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddSessionStateTempDataProvider();

#if DEBUG
            viewBuilder.AddRazorRuntimeCompilation();
#endif

            services.AddRazorPages();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.EnableAnnotations();
            });

            // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
            services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);

                // optional - default path to view services is /listallservices - recommended to choose your own path
                config.Path = "/listservices";
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //----

            services.AddIdentity<User, Role>().AddDefaultTokenProviders();
            services.AddSingleton<IUserStore<User>, UserStore>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, AppClaimsPrincipalFactory>();

            services.AddSingleton<IRoleStore<Role>, UserRoleStore>();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessDenied";
                options.SlidingExpiration = false;
                options.Cookie.Name = "App.Identity";
            });

            // Enable Dual Authentication 
            var jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(jwtIssuerOptions);
            var appSettings = jwtIssuerOptions.Get<JwtIssuerOptions>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthentication()
                //.AddFacebook(facebookOptions =>
                //{
                //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                //})
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = appSettings.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // var keyFolder = Directory.GetCurrentDirectory() + "/data/key/";
            // if (!Directory.Exists(keyFolder))
            // {
            //     Directory.CreateDirectory(keyFolder);
            // }

            services.AddDataProtection()
                // .PersistKeysToFileSystem(new DirectoryInfo(keyFolder))
                .SetApplicationName("subbynetwork")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            //-----


            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();
            services.AddAutoMapper(typeof(MauiProgram));
            services.Configure<SmtpConfig>(Configuration.GetSection(nameof(SmtpConfig)));
            services.AddScoped<ISendInBlue, SendInBlue>();
            services.AddScoped<IPushNotification, PushNotification>();
            services.AddScoped<IAppCache, AppCache>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<ICache, RedisCache>();
            services.AddScoped<IFileUpload, FileUpload>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddServiceBus(Configuration, Configuration.GetConnectionString("SchedulerConnection"));

            return builder.Build();
        }
    }
}