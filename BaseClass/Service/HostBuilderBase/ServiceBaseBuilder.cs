using BaseClass.Base;
using BaseClass.Base.Interface;
using BaseClass.Service.BaseWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseClass.Service.HostBuilderBase
{
    public static class ServiceBaseBuilder
    {
        public static IHostBuilder CreateServiceBase<TService, TWorker>(this IHostBuilder host)
            where TService : ServiceBase
            where TWorker : ServiceWorkerBase
        {
            host.ConfigureServices((host, services) =>
            {
                services.AddHostedService<TWorker>();
                services.AddSingleton<IBaseProvider, BaseProvider>();
                services.AddTransient<ServiceBase, TService>();
            });
            return host;
        }
    }
}
