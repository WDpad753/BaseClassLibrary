using BaseClass.Base;
using BaseClass.Base.Interface;
using BaseClass.ConsoleAppBase.BaseWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseClass.ConsoleAppBase.HostBuilderBase
{
    public static class ConsoleBaseBuilder
    {
        public static IHostBuilder CreateConsoleBase<TConsole, TWorker>(this IHostBuilder host)
            where TConsole : ConsoleBase
            where TWorker : ConsoleWorkerBase
        {
            host.ConfigureServices((host, services) =>
            {
                services.AddHostedService<TWorker>();
                services.AddSingleton<IBaseProvider, BaseProvider>();
                services.AddTransient<ConsoleBase, TConsole>();
            });
            return host;
        }
    }
}
