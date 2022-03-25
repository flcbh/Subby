using Autofac;
using Subby.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Subby.Infrastructure.Middlewares;
using Subby.Utilities.Authentication;
using Subby.Utilities.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Subby.Core.Entities;
using Subby.Core.Mappings;
using Subby.Infrastructure.Data;
using LastContent.Middleware.Logging;
using LastContent.ServiceBus.Core;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Email;
using LastContent.Utilities.Notification;
using Microsoft.AspNetCore.DataProtection;
using Subby.Core.Interfaces;
using Subby.Core.Services;
using Subby.Infrastructure.Factory;
using Subby.Infrastructure.UserStores;
using Subby.Utilities.Interfaces;
using Subby.Blazor.Maui.Extensions;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;


namespace Subby.Blazor.Maui
{
    public class Startup : IStartup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            Configuration = config;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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
            ConfigureAuthentication(services);
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();
            services.AddAutoMapper(typeof(Startup));
            services.Configure<SmtpConfig>(Configuration.GetSection(nameof(SmtpConfig)));
            services.AddScoped<ISendInBlue, SendInBlue>();
            services.AddScoped<IPushNotification, PushNotification>();
            services.AddScoped<IAppCache, AppCache>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<ICache, RedisCache>();
            services.AddScoped<IFileUpload, FileUpload>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddServiceBus(Configuration, Configuration.GetConnectionString("SchedulerConnection"));
        }

        private void ConfigureAuthentication(IServiceCollection services)
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

            // Enable Dual Authentication 
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

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DefaultInfrastructureModule(_env.EnvironmentName == "Development"));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseShowAllServicesMiddleware();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseApiLoggingMiddleware();
                app.UseHsts();
            }

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

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            //appBuilder
            //    .RegisterBlazorMauiWebView()
            //    .UseMicrosoftExtensionsServiceProviderFactory()
            //    .UseMauiApp<App>()
            //    .ConfigureFonts(fonts =>
            //    {
            //        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            //    })
            //    .ConfigureServices(services =>
            //    {
            //        services.AddBlazorWebView();
            //        services.AddSingleton<WeatherForecastService>();
            //    });
        }
        //public void Configure(IAppHostBuilder appBuilder)
        //{
        //    appBuilder
        //        .RegisterBlazorMauiWebView()
        //        .UseMicrosoftExtensionsServiceProviderFactory()
        //        .UseMauiApp<App>()
        //        .ConfigureFonts(fonts =>
        //        {
        //            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        //        })
        //        .ConfigureServices(services =>
        //        {
        //            services.AddBlazorWebView();
        //            services.AddSingleton<WeatherForecastService>();
        //        });
        //}
    }
}