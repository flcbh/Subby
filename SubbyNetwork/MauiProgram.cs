using LastContent.ServiceBus.Core;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Email;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Subby.Data;
using SubbyNetwork.Interfaces;
using SubbyNetwork.Services;
using System.Reflection;

namespace SubbyNetwork
{
    public static class MauiProgram
    {
        public static IConfiguration _Configuration { get; set; }
        private static string _environmentName;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

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

            var connectionString = _Configuration.GetConnectionString("SQLConnection");
            builder.Services.AddDbContext<SubbynetworkContext>(x => x.UseSqlServer("Data Source=tcp:susby.database.windows.net,1433;Initial Catalog=subbynetwork;Persist Security Info=False;User ID=subbynetwork1;Password=Sustainability123;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;"));

            builder.Services.AddSingleton<SubbynetworkContext>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //builder.Services.Configure<SmtpConfig>(_Configuration.GetSection(nameof(SmtpConfig)));
            builder.Services.AddScoped<ISendEmail, SendEmail>();
            builder.Services.AddScoped<IPushNotification, PushNotification>();
            builder.Services.AddScoped<IAppCache, AppCache>();
            builder.Services.AddScoped<ICache, RedisCache>();
            builder.Services.AddScoped<IFileUpload, FileUpload>();
            //builder.Services.AddServiceBus(builder.Configuration, builder.Configuration.GetConnectionString("SchedulerConnection"));


            return builder.Build();
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