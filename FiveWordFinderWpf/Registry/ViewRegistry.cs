using Microsoft.Extensions.DependencyInjection;
using FiveWordFinderWpf.View;

namespace FiveWordFinderWpf.Registry
{
    internal static class ViewRegistry
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddScoped<GraphFileView>();
            services.AddScoped<HomeView>();
        }
    }
}
