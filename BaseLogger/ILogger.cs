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
        public void Base(string message);
        public void Alert(string message);
        public void Debug(string message);
        public void Info(string message);
        public void Error(string message);
    }

    //public interface ILogger<T> : ILogger { }
}
