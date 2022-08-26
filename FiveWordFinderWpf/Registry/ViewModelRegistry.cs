using FiveWordFinderWpf.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace FiveWordFinderWpf.Registry
{
    internal static class ViewModelRegistry
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<EvaluateGraphViewModel>();
            services.AddScoped<GraphFileViewModel>();
            services.AddScoped<HomeViewModel>();
        }
    }
}
