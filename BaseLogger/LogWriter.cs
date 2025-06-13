using Common.Abstractions;
using Common.Abstractions.Models;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

namespace BaseLogger
{
    public class LogWriter : ILogWriter
    {
        string logtext;
        private ExeConfigurationFileMap _fileMap;
        private int? loggingLvl;
        private int? debugState;
        private string? appName;
        private string? logFilePath;

        public LogWriter(string configPath, string? logfilePath)
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
                LogWrite("Log directory does not exist, created directory to save log files.", this.GetType().Name, nameof(LogWriter), MessageLevels.Trace);
            }
            else
            {
                logFilePath=logfilePath;
            }

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Path.GetDirectoryName(configPath);
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Changed += ConfigOnChanged;
                watcher.Filter = "*.config";
            }
        }

        private void ConfigOnChanged(object sender, FileSystemEventArgs e)
        {
            appName = (string?)SetupConfigReader(typeof(string), "AppName");
            loggingLvl = (int?)SetupConfigReader(typeof(int), "LoggingLevel");
            debugState = (int?)SetupConfigReader(typeof(int), "DebugState");
        }

        public void LogWrite(string message, string appbase, string func, MessageLevels Messagelvl)
        {
            // Initialising variables:
            string filename = $"{appName}";
            string? filenamepath = logFilePath;
            DebugState debugLevel = debugState != null ? (DebugState)debugState : DebugState.Inactive;

            if (filenamepath == null)
            {
                throw new ArgumentNullException("filename");
            }

            DateTime datetime = DateTime.Now;
            string datetimenw = datetime.ToString("dddd, yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            StringBuilder sb = new StringBuilder();
            int LoggingState = 0;
            loggingLvl = loggingLvl != null ? loggingLvl : 1;

            if ((int)Messagelvl > loggingLvl && (int)debugLevel == 0)
                return;
            else if ((int)Messagelvl > loggingLvl && (int)debugLevel == 1)
            {
                logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {appbase}::{func} - {message}";

                sb.Append(logtext);

                // Displaying the logs into the Console and Debug Section:
                Debug.WriteLine(sb.ToString());
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
            else
            {
                if (loggingLvl > (int)Enum.GetValues(typeof(MessageLevels)).Cast<MessageLevels>().Last())
                {

                    logtext = $"{appName}: {datetimenw} LevelX: {appbase}::{func} - Unknown Log Level, Please change the log level.";

                    sb.Append(logtext);

                    // Saving the logs into the textfile:
                    File.AppendAllText(filenamepath, sb.ToString() + Environment.NewLine);
                    Console.WriteLine(sb.ToString());
                    sb.Clear();
                    return;
                }

                if (debugState == 0)
                {
                    logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {appbase}::{func} - {message}";

                    sb.Append(logtext);

                    // Saving the logs into the textfile:
                    File.AppendAllText(filenamepath, sb.ToString() + Environment.NewLine);
                    sb.Clear();
                }
                else
                {
                    logtext = $"{appName}: {datetimenw} Level{Messagelvl.ToString().Substring(0, 1)}: {appbase}::{func} - {message}";

                    sb.Append(logtext);

                    // Saving the logs into the textfile:
                    Debug.WriteLine(sb.ToString());
                    Console.WriteLine(sb.ToString());
                    sb.Clear();
                }
            }
        }

        private object SetupConfigReader(Type expectedType, string path)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

            string value = config.AppSettings.Settings[path].Value;

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
    }
}
