using Common.Abstractions.Models;

namespace Common.Abstractions
{
    public interface ILogWriter
    {
        void LogWrite(string message, string servicebase, string func, MessageLevels Messagelvl, DebugState debugLevel = 0);

        //public string? LogFilePath { get; set; }
        //public string? ConfigFilePath { get; set; }
    }
}
