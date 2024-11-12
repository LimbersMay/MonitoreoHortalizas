using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.services;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using MonitoreoHortalizasApp.Models;

namespace MonitoreoHortalizasApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public void OnStartup(object sender, StartupEventArgs e)
        { var services = new ServiceCollection();
            ConfigureServices(services);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServiceProvider = services.BuildServiceProvider();
            
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            
            services.AddSingleton(Configuration);
            
            // WPF Dependencies
            services.AddWpfBlazorWebView();
            services.AddBlazorBootstrap();
            
            services.AddTransient(typeof(MainWindow));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(Configuration);
            });

            services.AddSingleton<ISerialPortReader, SerialPortReader>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddTransient<IBedRepository, BedRepository>();
            services.AddTransient<IJsonParser, JsonParser>();
            services.AddTransient<ITemperatureRepository, TemperatureRepository>();
            services.AddTransient<IBarometricRepository, BarometricRepository>();
            services.AddTransient<IValveRepository, ValveRepository>();
            services.AddSingleton<TabHeaderService>();
            services.AddSingleton<Settings>();
            services.AddTransient<Runner>();
        }
    }
}
