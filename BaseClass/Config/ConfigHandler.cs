using BaseClass.Helper;
using BaseClass.JSON;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Config
{
    public class ConfigHandler
    {
        //private string? configPath;
        private ExeConfigurationFileMap _fileMap;
        private EnvFileReader _envFileReader;
        private string? _filepath;
        //private ILogWriter _logWriter;
        private LogWriter _logWriter;
        private AppSettingsSection? _configAppSettingsSection;
        private loggerSettings? _configLoggerSettingsSection;
        public bool _ConfigRead = false;
        //private JSONFileHandler? _fileHandler;

        public ConfigHandler(string? filepath, LogWriter Logger)
        {
            _filepath = filepath;
            _logWriter = Logger;

            _fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = filepath,
            };

            _envFileReader = new(Logger);
        }

        public void SaveInfo(string data, string path, string? section = null)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section == null || section.ToString().Contains("appSettings"))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection("appSettings");
                }
                else if(section.ToString().Contains("loggerSettings"))
                {
                    _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");
                }
                else
                {
                    _ConfigRead = false;
                    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                    return;
                }

                if (_configAppSettingsSection != null)
                {
                    _configAppSettingsSection.CurrentConfiguration.AppSettings.Settings[path].Value = data;

                    // Save the modified configuration
                    config.Save(ConfigurationSaveMode.Modified);

                    //Refresh the section
                    ConfigurationManager.RefreshSection("appSettings");

                    _logWriter.LogWrite($"{data} was saved in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Main);

                    _logWriter.LogWrite($"{data} was saved in {path} Key in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug);
                }

                else if (_configLoggerSettingsSection != null)
                {
                    //_configLoggerSettingsSection.LoggerSettings[path]?.value = data;

                    //// Save the modified configuration
                    //config.Save(ConfigurationSaveMode.Modified);

                    ////Refresh the section
                    //ConfigurationManager.RefreshSection("appSettings");

                    _logWriter.LogWrite($"{data} was saved in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Main);

                    _logWriter.LogWrite($"{data} was saved in {path} Key in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug);
                }
                else
                {
                    _logWriter.LogWrite("Unable to save the data in the config file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
            }
        }

        public string? ReadInfo(string path, string? section = null)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section == null || section.ToString().Contains("appSettings"))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection("appSettings");
                }
                else if (section.ToString().Contains("loggerSettings"))
                {
                    _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");
                }
                else
                {
                    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                    return null;
                }

                if (_configAppSettingsSection != null)
                {
                    string data = _configAppSettingsSection.CurrentConfiguration.AppSettings.Settings[path].Value;

                    _logWriter.LogWrite($"{data} was collected from {path} Key in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug);

                    return data;
                }
                else if(_configLoggerSettingsSection != null)
                {
                    string data = _configLoggerSettingsSection.LoggerSettings[path]?.value;

                    _logWriter.LogWrite($"{data} was collected from {path} Key in Config File.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug);

                    return data;
                }
                else
                {
                    _logWriter.LogWrite("Unable to save the data in the config file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        public string? EnvRead(string path, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            string? data = null;

            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    _logWriter.LogWrite($"Was not able to obtain value from given path.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Trace);
                    _logWriter.LogWrite($"Was not able to obtain value from given path. Submitted path => {path}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug);
                    return null;
                }

                if (mode == null || mode == EnvAccessMode.Project)
                {
                    // Read environment variable from the current process scope
                    data = Environment.GetEnvironmentVariable(path);
                }
                else if (mode == EnvAccessMode.File)
                {
                    data = _envFileReader.EnvFileRead(envpath, path, envkeyname);
                }
                else if (mode == EnvAccessMode.User)
                {
                    data = Environment.GetEnvironmentVariable(path, EnvironmentVariableTarget.User);
                }
                else if (mode == EnvAccessMode.System)
                {
                    data = Environment.GetEnvironmentVariable(path, EnvironmentVariableTarget.Machine);
                }

                if(data == null)
                {
                    _logWriter.LogWrite($"Unable to obtain value from given path => {path}.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);

                    return null;
                }
                else
                {
                    _logWriter.LogWrite($"Obtained following value {data} from given path => {path}.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Main);

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }
    }
}
