namespace Subby.Blazor.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var route = "Views/Adverts/Index.cshtml";

            WebView webView = new WebView
            {
                Source = route
            };

            //webView.Focus();
            //Shell.Current.GoToAsync(route);
        }
    }
}