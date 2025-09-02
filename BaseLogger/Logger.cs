using BaseLogger.Models;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace BaseLogger
{
    public class Logger: ILogger
    {
        string? logtext;
        private ExeConfigurationFileMap _fileMap;
        private int? loggingLvl;
        private int? debugState;
        private string? appName;
        private string? baseMsg;
        private string logFilePath;
        private FileSystemWatcher watcher;
        private loggerSettings? _configLoggerSettingsSection;
        private static readonly object _lockObj = new object();


        public Logger(string configPath, string logfilePath)
        {
            _fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configPath,
            };

            appName = (string?)SetupConfigReader(typeof(string), "AppName");
            loggingLvl = (int?)SetupConfigReader(typeof(int), "LoggingLevel");
            debugState = (int?)SetupConfigReader(typeof(int), "DebugState");
            
            if (Directory.Exists(logfilePath) == false)
            {
                Directory.CreateDirectory(logfilePath);
                logFilePath=logfilePath;
                lock(_lockObj)
                {
                    //LogWrite("Log directory does not exist, created directory to save log files.", this.GetType().Name, nameof(LogWriter), MessageLevels.Verbose);
                    LogAlert("Log directory does not exist, created directory to save log files.");
                }
            }
            else
            {
                logFilePath=logfilePath;
            }

            watcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(configPath),
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.config"
            };

            watcher.Changed += ConfigOnChanged;
            //watcher.Renamed += ConfigOnChanged;
            //watcher.Created += ConfigOnChanged;
        }

        public void LogInfo(string message) => LogWrite(message, FuncName.GetMethodName(), MessageLevels.Info);

        public void LogAlert(string message) => LogWrite(message, FuncName.GetMethodName(), MessageLevels.Alert);

        public void LogError(string message) => LogWrite(message, FuncName.GetMethodName(), MessageLevels.Fatal);

        public void LogDebug(string message) => LogWrite(message, FuncName.GetMethodName(), MessageLevels.Debug);

        public void LogBase(string message) => LogWrite(message, FuncName.GetMethodName(), MessageLevels.Base);

        public void Log(string message, string func, MessageLevels level) => LogWrite(message, FuncName.GetMethodName(), level);

        private void ConfigOnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                string filenamepath = Path.Combine(logFilePath, appName+".log");

                Debug.WriteLine($"[Watcher] Event: {e.ChangeType} on {e.FullPath} at {DateTime.Now:HH:mm:ss.fff}");

                if(debugState == 0)
                {
                    string logtext = $"[Watcher] Event: {e.ChangeType} on {e.FullPath} at {DateTime.Now:HH:mm:ss.fff}";
                    lock (_lockObj)
                    {
                        File.AppendAllText(filenamepath, logtext + Environment.NewLine);
                    }
                }

                // existing reload logic...
                appName = (string?)SetupConfigReader(typeof(string), "AppName");
                loggingLvl = (int?)SetupConfigReader(typeof(int), "LoggingLevel");
                debugState = (int?)SetupConfigReader(typeof(int), "DebugState");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Watcher] Exception in handler: {ex}");
            }
        }

        private void LogWrite(string message, string func, MessageLevels Messagelvl)
        {
            // Initialising variables:
            DebugState debugLevel = debugState != null ? (DebugState)debugState : DebugState.Inactive;
            baseMsg = FuncName.GetCallingClassName();

            baseMsg = baseMsg == null ? "" : baseMsg;

            string filenamepath = Path.Combine(logFilePath, appName+".log");

            if (filenamepath == null)
            {
                throw new ArgumentNullException("filename");
            }

            DateTime datetime = DateTime.Now;
            string datetimenw = datetime.ToString("dddd, yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            loggingLvl = loggingLvl != null ? loggingLvl : 1;

            if ((int)Messagelvl > loggingLvl && (int)debugLevel == 0)
                return;
            else if ((int)Messagelvl > loggingLvl && (int)debugLevel == 1)
            {
                logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {baseMsg}::{func} - {message}";

                // Displaying the logs into the Console and Debug Section:
                Debug.WriteLine(logtext + Environment.NewLine);
                ConsoleLogWriter(appName, datetimenw, Messagelvl, baseMsg, func, message);
                //Console.WriteLine(logtext + Environment.NewLine);
            }
            else
            {
                if (loggingLvl > (int)Enum.GetValues(typeof(MessageLevels)).Cast<MessageLevels>().Last())
                {
                    string faultmessage = "Unknown Log Level, Please change the log level.";
                    logtext = $"{appName}: {datetimenw} Level{MessageLevels.XNull.ToString().Substring(0, 1)}: {baseMsg}::{func} - {faultmessage}";

                    if (debugState == 0)
                    {
                        // Saving the logs into the textfile:
                        lock (_lockObj)
                        {
                            File.AppendAllText(filenamepath, logtext + Environment.NewLine);
                        }
                    }

                    // Displaying the logs into the Console and Debug Section:
                    Debug.WriteLine(logtext + Environment.NewLine);
                    ConsoleLogWriter(appName, datetimenw, MessageLevels.XNull, baseMsg, func, faultmessage);
                    //Console.WriteLine(logtext + Environment.NewLine);
                    return;
                }

                if (debugLevel == 0)
                {
                    logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {baseMsg}::{func} - {message}";

                    // Saving the logs into the textfile:
                    lock (_lockObj)
                    {
                        File.AppendAllText(filenamepath, logtext + Environment.NewLine);
                    }
                }
                else
                {
                    logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {baseMsg}::{func} - {message}";
                    
                    // Displaying the logs into the Console and Debug Section:
                    Debug.WriteLine(logtext + Environment.NewLine);
                    ConsoleLogWriter(appName, datetimenw, Messagelvl, baseMsg, func, message);
                    //Console.WriteLine(logtext + Environment.NewLine);
                }
            }
        }

        private object SetupConfigReader(Type expectedType, string path)
        {
            string? value = null;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

            _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");

            if(_configLoggerSettingsSection != null)
            {
                value = _configLoggerSettingsSection.LoggerSettings[path]?.value;
            }
            else
            {
                value = config.AppSettings.Settings[path].Value;
            }

            try
            {
                if (expectedType == typeof(int))
                    return int.Parse(value);
                if (expectedType == typeof(string))
                    return value;

                throw new Exception("Type not supported.");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format($"Config key:{path} was expected to be of type {expectedType} but was not."), ex);
            }
        }

        private void ConsoleLogWriter(string? AppName, string? DatetimeNow, MessageLevels MessageLvl, string? BaseMessage, string? Function, string? Message)
        {
            string FirstSec = $"{AppName}: {DatetimeNow} ";
            string LevelTag = $"Level{MessageLvl.ToString().Substring(0, 1)}";
            string SecondSec = $": {BaseMessage}::{Function} - {Message}";

            Console.Write(FirstSec);
            Console.ForegroundColor = SetConsoleColor(MessageLvl);
            Console.Write(LevelTag);
            Console.ResetColor();
            Console.WriteLine(SecondSec);
        }

        private ConsoleColor SetConsoleColor(MessageLevels messageLevel)
        {
            switch (messageLevel)
            {
                case MessageLevels.Fatal:
                    return ConsoleColor.DarkRed;
                case MessageLevels.Base:
                    return ConsoleColor.Red;
                case MessageLevels.Alert:
                    return ConsoleColor.Yellow;
                case MessageLevels.Debug:
                    return ConsoleColor.Magenta;
                case MessageLevels.Info:
                    return ConsoleColor.Cyan;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}