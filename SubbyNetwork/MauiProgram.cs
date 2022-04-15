using LastContent.ServiceBus.Core;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Email;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Subby.Data;
using SubbyNetwork.Interfaces;
using SubbyNetwork.Models;
using SubbyNetwork.Services;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;

namespace SubbyNetwork
{
    public static class MauiProgram
    {
        public static IConfiguration _Configuration { get; set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var appSettings = ReadSettings();

            var assembly = Assembly.GetExecutingAssembly();
            var resName = assembly.GetManifestResourceNames()
                ?.FirstOrDefault(r => r.EndsWith("settings.json", StringComparison.OrdinalIgnoreCase));
            using var file = assembly.GetManifestResourceStream(resName);
            builder.Configuration.AddJsonStream(file);


            //#if __ANDROID__
            //            var documentsFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            //            _Configuration = new ConfigurationBuilder()
            //                   .SetBasePath(documentsFolderPath)
            //                   .AddJsonFile("appsettings.json")
            //                   .AddJsonFile($"appsettings.{_environmentName}.json", optional: true)
            //                   .Build();
            //#endif

            //#if WINDOWS10_0_17763_0_OR_GREATER

            //                    Assembly callingAssembly = Assembly.GetEntryAssembly();
            //                    Version versionRuntime = callingAssembly.GetName().Version;
            //                    string assemblyLocation = Path.GetDirectoryName(System.AppContext.BaseDirectory); //CallingAssembly.Location
            //                    var configFile = Path.Combine(assemblyLocation, "appsettings.json");
            //            _Configuration = new ConfigurationBuilder()
            //                   //.SetBasePath(Directory.GetCurrentDirectory())
            //                   //.AddJsonFile("appsettings.json")
            //                   .AddJsonFile(configFile, optional: false, reloadOnChange: true)
            //                   .Build();
            //#endif

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddMauiBlazorWebView();

            //Register needed elements for authentication
            builder.Services.AddAuthorizationCore(); // This is the core functionality
            builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // This is our custom provider
            //When asking for the default Microsoft one, give ours!
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());

            var connectionString = appSettings.SelectToken("ConnectionStrings").SelectToken("SQLConnection").Value<string>();
            var schedulerConnection = appSettings.SelectToken("ConnectionStrings").SelectToken("SchedulerConnection").Value<string>();

            builder.Services.AddDbContext<SubbynetworkContext>(x => x.UseSqlServer(connectionString));

            builder.Services.AddSingleton<SubbynetworkContext>();


            SmtpConfig smtpConfig = new SmtpConfig();

            smtpConfig.Port = appSettings.SelectToken("SmtpConfig").SelectToken("Port").Value<int>();
            smtpConfig.Server = appSettings.SelectToken("SmtpConfig").SelectToken("Server").Value<string>();
            smtpConfig.Pass = appSettings.SelectToken("SmtpConfig").SelectToken("Pass").Value<string>();
            smtpConfig.User = appSettings.SelectToken("SmtpConfig").SelectToken("User").Value<string>();
            smtpConfig.From = appSettings.SelectToken("SmtpConfig").SelectToken("From").Value<string>();
            smtpConfig.Live = appSettings.SelectToken("SmtpConfig").SelectToken("live").Value<bool>();

            ConfigureAuthentication(builder);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(nameof(SmtpConfig)));
            builder.Services.AddScoped<ISendEmail, SendEmail>();
            builder.Services.AddScoped<IPushNotification, PushNotification>();
            builder.Services.AddScoped<IAppCache, AppCache>();
            builder.Services.AddScoped<ICache, RedisCache>();
            builder.Services.AddScoped<IFileUpload, FileUpload>();
            //builder.Services.AddServiceBus(builder.Configuration, schedulerConnection);


            return builder.Build();
        }

        private static void ConfigureAuthentication(MauiAppBuilder builder)
        {
            builder.Services.AddIdentity<User, Role>().AddDefaultTokenProviders();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, AppClaimsPrincipalFactory>();

            builder.Services.Configure<IdentityOptions>(options =>
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

#if WINDOWS10_0_17763_0_OR_GREATER

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/accessDenied";
                options.SlidingExpiration = false;
                options.Cookie.Name = "App.Identity";
            });

#endif

            // Enable Dual Authentication 
            var jwtIssuerOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));
            builder.Services.Configure<JwtIssuerOptions>(jwtIssuerOptions);
            var appSettings = jwtIssuerOptions.Get<JwtIssuerOptions>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            builder.Services.AddAuthentication();
                //.AddFacebook(facebookOptions =>
                //{
                //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                //})
                //.AddJwtBearer(x =>
                //{
                //    x.RequireHttpsMetadata = false;
                //    x.SaveToken = true;
                //    x.TokenValidationParameters = new TokenValidationParameters
                //    {
                //        ValidateIssuerSigningKey = true,
                //        IssuerSigningKey = new SymmetricSecurityKey(key),
                //        ValidateIssuer = true,
                //        ValidIssuer = appSettings.Issuer,
                //        ValidateAudience = false,
                //        ValidateLifetime = true,
                //        ClockSkew = TimeSpan.Zero
                //    };
                //    x.Events = new JwtBearerEvents
                //    {
                //        OnAuthenticationFailed = context =>
                //        {
                //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                //            {
                //                context.Response.Headers.Add("Token-Expired", "true");
                //            }
                //            return Task.CompletedTask;
                //        }
                //    };
                //});

            // var keyFolder = Directory.GetCurrentDirectory() + "/data/key/";
            // if (!Directory.Exists(keyFolder))
            // {
            //     Directory.CreateDirectory(keyFolder);
            // }

            builder.Services.AddDataProtection()
                // .PersistKeysToFileSystem(new DirectoryInfo(keyFolder))
                .SetApplicationName("subbynetwork")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        }

        public static JObject ReadSettings()
        {
            // Get the assembly this code is executing in
            var assembly = Assembly.GetExecutingAssembly();

            // Look up the resource names and find the one that ends with settings.json
            // Your resource names will generally be prefixed with the assembly's default namespace
            // so you can short circuit this with the known full name if you wish
            var resName = assembly.GetManifestResourceNames()
                ?.FirstOrDefault(r => r.EndsWith("settings.json", StringComparison.OrdinalIgnoreCase));

            // Load the resource file
            using var file = assembly.GetManifestResourceStream(resName);

            // Stream reader to read the whole file
            using var sr = new StreamReader(file);

            // Read the json from the file
            var json = sr.ReadToEnd();

            // Parse out the JSON
            var j = JObject.Parse(json);

            return j;
        }
    }

    public class Settings
    {
        public int KeyOne { get; set; }
        public bool KeyTwo { get; set; }
        public NestedSettings KeyThree { get; set; } = null!;
    }

    public class NestedSettings
    {
        public string Message { get; set; } = null!;
    }
}