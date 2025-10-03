using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.ConsoleAppBase.BaseWorker
{
    public abstract class ConsoleWorkerBase : IHostedService
    {
        private Task? _executingTask;
        private readonly CancellationTokenSource _cts = new();

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_cts.Token);

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            }
        }
    }
}
