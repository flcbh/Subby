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
            
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            builder.Services.AddBlazorWebView();
            builder.Services.AddSingleton<WeatherForecastService>();

            builder.Services.AddNHibernate(
               Configuration.GetConnectionString("SQLConnection"),
               Assembly.GetAssembly(typeof(Session)),
               Assembly.GetAssembly(typeof(SessionMappingOverride)));

            builder.Services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisCache");
            });

            builder.Services.AddHealthChecks();
            builder.Services.AddControllersWithViews().AddNewtonsoftJson();

            var viewBuilder = builder.Services.AddControllersWithViews(options => { })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                ).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AccountController>())
                .AddSessionStateTempDataProvider();

#if DEBUG
            viewBuilder.AddRazorRuntimeCompilation();
#endif

            builder.Services.AddRazorPages();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.EnableAnnotations();
            });

            // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
            builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(builder.Services);

                // optional - default path to view services is /listallservices - recommended to choose your own path
                config.Path = "/listservices";
            });


            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            ConfigureAuthentication(builder.Services);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSession();
            builder.Services.AddAutoMapper(typeof(AccountController));
            builder.Services.Configure<SmtpConfig>(Configuration.GetSection(nameof(SmtpConfig)));
            builder.Services.AddScoped<ISendInBlue, SendInBlue>();
            builder.Services.AddScoped<IPushNotification, PushNotification>();
            builder.Services.AddScoped<IAppCache, AppCache>();
            builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            builder.Services.AddScoped<ICache, RedisCache>();
            builder.Services.AddScoped<IFileUpload, FileUpload>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddServiceBus(Configuration, Configuration.GetConnectionString("SchedulerConnection"));

            return builder.Build();
        }

        private static void ConfigureAuthentication(IServiceCollection services)
        {
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

            //Enable Dual Authentication
            var jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(jwtIssuerOptions);
            var appSettings = jwtIssuerOptions.Get<JwtIssuerOptions>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthentication()
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

            services.AddDataProtection()
                // .PersistKeysToFileSystem(new DirectoryInfo(keyFolder))
                .SetApplicationName("subbynetwork")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        }

        //public static void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new DefaultInfrastructureModule(_env.EnvironmentName == "Development"));
        //}


        public static void Configure(IApplicationBuilder app)
        {
            //if (_env.EnvironmentName == "Development")
            //{
            //    app.UseBrowserLink();
            //    app.UseDeveloperExceptionPage();
            //    //app.UseShowAllServicesMiddleware();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //    app.UseApiLoggingMiddleware();
            //    app.UseHsts();
            //}

            app.UseLoggingMiddleware();
            app.UseExceptionMiddleware();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

    }
}