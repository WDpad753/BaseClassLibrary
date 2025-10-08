using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Service.BaseWorker
{
    public abstract class ServiceWorkerBase : BackgroundService
    {
        private readonly ServiceBase serviceBase;
        private readonly IHostApplicationLifetime host;

        public ServiceWorkerBase(IHostApplicationLifetime Host, ServiceBase service)
        {
            serviceBase = service;
            host = Host;
        }

        protected abstract Task ExecuteTaskAsync(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ExecuteTaskAsync(stoppingToken);
        }
    }
}
