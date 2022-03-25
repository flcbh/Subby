using System;
using Ardalis.ListStartupServices;
using Autofac;
using Subby.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
using Subby.Web.Extensions;
using Microsoft.AspNetCore.Rewrite;

namespace Subby.Web
{
    public class Startup
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
            // services.Configure<CookiePolicyOptions>(options =>
            // {
            //     options.CheckConsentNeeded = context => true;
            //     options.MinimumSameSitePolicy = SameSiteMode.None;
            // });
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
                c.EnableAnnotations();
            });

            // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
            services.Configure<ServiceConfig>(config =>
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
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
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
                app.UseShowAllServicesMiddleware();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseApiLoggingMiddleware();
                app.UseHsts();


                /*
                app.UseRewriter(new RewriteOptions()
                  .AddRedirectToNonWww()
                  .AddRedirectToHttps()
               );
                app.Use(async (context, next) =>
                {
                    var url = context.Request.Host.Value;

                    // Redirect to an external URL
                    if (url.Contains("subbynetwork"))
                    {
                        url = url.Replace("subbynetwork", "sustainabilityyard");
                        context.Response.Redirect(url);
                        return;   // short circuit
                    }

                    await next();
                });*/
            }

            app.UseLoggingMiddleware();
            app.UseExceptionMiddleware();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // app.UseCookiePolicy();
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