using BaseClass.Base.Interface;
using BaseClass.Database.Factory;
using BaseClass.Database.Interface;
using BaseClass.Model;
using BaseLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BaseClass.Service
{
    public class BaseConfigException : ApplicationException
    {
        public BaseConfigException() : base() { }

        public BaseConfigException(string? message) : base(message) { }

        public BaseConfigException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public abstract class ServiceBase : IDisposable
    {
        private readonly IBaseProvider? _provider;
        private readonly DatabaseMode? dbMode;
        private bool _disposedValue;

        protected string? ConsoleAppName { get; set; }
        protected CancellationToken CancellationToken { get; set; }
        protected ILogger? Logger { get; private set; }
        protected IBaseSettings Settings { get; private set; }
        protected IDatabase? DB { get; private set; }
        protected IDatabase? DBLite { get; private set; }

        protected ServiceBase(IBaseProvider provider)
        {
            _provider = provider;
            Logger = _provider?.GetItem<ILogger>() ?? throw new BaseConfigException("Logger Not Configured");
            Settings = _provider?.GetItem<IBaseSettings>() ?? throw new BaseConfigException("Settings Not Configured");
            ConsoleAppName = _provider?.GetValue<string>("ConsoleName") ?? throw new BaseConfigException("Console Name is not Configured");
            dbMode = _provider.GetValue<DatabaseMode>("DatabaseMode");

            Logger.LogAlert($"{new string('=', 30)}");
            Logger.LogAlert($" - entry {ConsoleAppName}");

            if (dbMode != null)
            {
                if (dbMode == DatabaseMode.SQLServer)
                {
                    CreateServerDatabase();
                }
                else if (dbMode == DatabaseMode.SQLite)
                {
                    CreateLiteDatabase();
                }
                else
                {
                    throw new BaseConfigException("Selected Database Mode does not Exist.");
                }
            }
            else
            {
                Logger.LogInfo($" - Database Access is not setup.");
            }
        }

        public abstract bool CanStart();
        public virtual async Task Start(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            await StartupTasks();
        }
        public abstract Task StartupTasks();

        [MemberNotNull(nameof(DB))]
        protected void CreateServerDatabase()
        {
            DB = DatabaseFactory.GetDatabase((DatabaseMode)dbMode, Settings);
        }

        [MemberNotNull(nameof(DBLite))]
        protected void CreateLiteDatabase()
        {
            DBLite = DatabaseFactory.GetDatabase((DatabaseMode)dbMode, Settings);
        }

        protected static Timer CreateTimer(int interval, ElapsedEventHandler handler)
        {
            Timer timer = new()
            {
                Interval = interval,
                Enabled = false,
                AutoReset = true
            };
            timer.Elapsed += handler;
            return timer;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ServiceBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
