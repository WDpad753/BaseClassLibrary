//using Common.Abstractions;
//using Common.Abstractions.Models;
using BaseLogger.Models;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

namespace BaseLogger
{
    public class LogWriter
    {
        string? logtext;
        private ExeConfigurationFileMap _fileMap;
        private int? loggingLvl;
        private int? debugState;
        private string? appName;
        private string logFilePath;
        private FileSystemWatcher watcher;
        private loggerSettings? _configLoggerSettingsSection;

        public LogWriter(string configPath, string logfilePath)
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

            watcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(configPath),
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.config"
                //Filter = Path.GetFileName(configPath)
            };

            watcher.Changed += ConfigOnChanged;
            //watcher.Renamed += ConfigOnChanged;
            //watcher.Created += ConfigOnChanged;
        }

        private void ConfigOnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                Debug.WriteLine($"[Watcher] Event: {e.ChangeType} on {e.FullPath} at {DateTime.Now:HH:mm:ss.fff}");
                // existing reload logic...
                //appName = (string?)SetupConfigReader(typeof(string), "AppName");
                loggingLvl = (int?)SetupConfigReader(typeof(int), "LoggingLevel");
                debugState = (int?)SetupConfigReader(typeof(int), "DebugState");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Watcher] Exception in handler: {ex}");
            }
        }

        public void LogWrite(string message, string? appbase, string func, MessageLevels Messagelvl)
        {
            // Initialising variables:
            string filename = $"{appName}";
            string? filenamepath = Path.Combine(logFilePath,filename+".log");
            DebugState debugLevel = debugState != null ? (DebugState)debugState : DebugState.Inactive;
            appbase = appbase == null ? "" : appbase;

            if (filenamepath == null)
            {
                throw new ArgumentNullException("filename");
            }

            DateTime datetime = DateTime.Now;
            string datetimenw = datetime.ToString("dddd, yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            StringBuilder sb = new StringBuilder();
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
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

            //string value = config.AppSettings.Settings[path].Value;

            _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");


            string value = _configLoggerSettingsSection.LoggerSettings[path]?.value;

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