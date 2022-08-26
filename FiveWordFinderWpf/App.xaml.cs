using FiveWordFinderWpf.Model;
using FiveWordFinderWpf.Registry;
using FiveWordFinderWpf.Services;
using FiveWordFinderWpf.View;
using FiveWordFinderWpf.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FiveWordFinderWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ViewModelRegistry.ConfigureServices(services);
            ViewRegistry.ConfigureServices(services);
            services.AddScoped(typeof(INavigationService<>), typeof(NavigationService<>));
            services.AddScoped<ApplicationState>();

            services.AddTransient<FiveWordFinder.WordProcessing.IGraphGenerator,
                                  FiveWordFinder.WordProcessing.GraphGenerator>();
            services.AddTransient<FiveWordFinder.WordProcessing.Strategies.IGraphEvaluationNotify,
                                  FiveWordFinder.WordProcessing.Strategies.EvaluateGraphParallelNotify>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
        }
    }
}
