using BaseClass.Base;
using BaseClass.Base.Interface;
using BaseClass.ConsoleAppBase.BaseWorker;
using BaseClass.Service;
using BaseClass.Service.BaseWorker;
using BaseLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.ConsoleAppBase.HostBuilderBase
{
    public static class ConsoleBaseBuilder
    {
        public static BaseSettings baseSettings { get; } = new();

        public static IHostBuilder CreateConsoleBase<TConsole, TWorker>(this IHostBuilder host)
            where TConsole : ConsoleBase
            where TWorker : ConsoleWorkerBase
        {
            host.ConfigureServices((host, services) =>
            {
                // Get the current directory (where your executable is located):
                string currentDirectory = Directory.GetCurrentDirectory();
                string currentDirectory2 = AppDomain.CurrentDomain.BaseDirectory;

                string configFilePath = Path.Combine(currentDirectory2, "Config");
                string[] files = (string[])Directory.GetFiles(configFilePath, "*.config");
                bool val = Directory.Exists(configFilePath);

                // Double check
                if (!Directory.Exists(configFilePath) || files.Count() < 0)
                {
                    throw new Exception("Either the Config File Path does not exist or there are different Configs in the assigned path.");
                }

                string configFile = files[0];
                string logFilePath = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "tmp");

                //
                services.AddHostedService<TWorker>();
                services.AddSingleton<IBaseSettings>(baseSettings);
                services.AddSingleton<ILogger>(new Logger(configFile, logFilePath));
            });
            return host;
        }
    }
}
