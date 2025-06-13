using Common.Abstractions.Models;

namespace Common.Abstractions
{
    public interface ILogWriter
    {
        void LogWrite(string message, string servicebase, string func, MessageLevels Messagelvl);

        //public string? LogFilePath { get; set; }
        //public string? ConfigFilePath { get; set; }
    }
}
