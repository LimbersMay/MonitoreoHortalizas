using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using GestionHortalizasApp.entities;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Services;

namespace MonitoreoHortalizasApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MainWindow( IServiceProvider serviceProvider)
        {
            InitializeComponent();
            
            Resources.Add("services", serviceProvider);
            
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Runner>>();
                
                // Log the error in debug console
                System.Diagnostics.Debug.WriteLine(e.ExceptionObject.ToString());

                logger.LogCritical(e.ExceptionObject.ToString());
                Environment.Exit(1);
            };

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                var error = e.Exception.ToString();

                var logger = serviceProvider.GetRequiredService<ILogger<Runner>>();
                logger.LogCritical(error);

                System.Diagnostics.Debug.WriteLine(e.Exception.ToString());
            };
        }

        private void Handle_UrlLoading(object sender, UrlLoadingEventArgs urlLoadingEventArgs)
        {
            if (urlLoadingEventArgs.Url.Host != "0.0.0.0")
            {
                urlLoadingEventArgs.UrlLoadingStrategy =
                    UrlLoadingStrategy.OpenInWebView;
            }
        }
    }
}