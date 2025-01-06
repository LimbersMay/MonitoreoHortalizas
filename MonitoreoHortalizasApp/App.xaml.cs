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
using MonitoreoHortalizasApp.Source.SowingCycles.Services;

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
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            Console.WriteLine($"Environment: {environment}");
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

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
            services.AddTransient<IGerminationLogRepository, GerminationLogRepository>();
            services.AddTransient<ISowingCycleRepository, SowingCycleRepository>();
            services.AddTransient<ISowingRepository, SowingRepository>();
            services.AddTransient<ISowingLineRepository, SowingLineRepository>();
            services.AddTransient<IGenerateIdService, GenerateIdService>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles));
            services.AddTransient<Runner>();
            
            // Services to manage the state of the application
            services.AddSingleton<TabHeaderService>();
            services.AddSingleton<SowingCycleFormService>();
        }
    }
}
