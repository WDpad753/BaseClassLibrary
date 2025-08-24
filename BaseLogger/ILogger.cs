using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLogger
{
    public interface ILogger
    {
        public void Log(string message, string func, MessageLevels Messagelvl);
        public void LogBase(string message);
        public void LogAlert(string message);
        public void LogDebug(string message);
        public void LogInfo(string message);
        public void LogError(string message);
    }

    //public interface ILogger<T> : ILogger { }
}
