using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Subby.Data;
using SubbyNetwork.Data;
using SubbyNetwork.Services;

namespace SubbyNetwork
{
    public static class MauiProgram
    {
        public static IConfiguration Configuration { get; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            //Register needed elements for authentication
            builder.Services.AddAuthorizationCore(); // This is the core functionality
            builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // This is our custom provider
            //When asking for the default Microsoft one, give ours!
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());

            var connectionString = builder.Configuration.GetConnectionString("SQLConnection");
            builder.Services.AddDbContext<SubbynetworkContext>(x => x.UseSqlServer("Data Source=tcp:susby.database.windows.net,1433;Initial Catalog=subbynetwork;Persist Security Info=False;User ID=subbynetwork1;Password=Sustainability123;MultipleActiveResultSets=True;encrypt=false;"));

            builder.Services.AddBlazorWebView();
            builder.Services.AddSingleton<WeatherForecastService>();


            return builder.Build();
        }
    }
}