using BaseClass.JSON;
using BaseLogger;
using BaseLogger.Models;

//using Common.Abstractions;
//using Common.Abstractions.Models;
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
    public class ConfigReader
    {
        private string configPath;
        private ExeConfigurationFileMap _fileMap;
        private string? _filepath;
        //private ILogWriter _logWriter;
        private LogWriter _logWriter;
        private AppSettingsSection _configAppSettingsSection;
        public bool _ConfigRead = false;
        private JSONFileHandler _fileHandler;

        public ConfigReader(string? filepath, LogWriter Logger)
        {
            _filepath = filepath;
            _logWriter = Logger;

            _fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = filepath,
            };
        }

        public void SaveInfo(string data, string path, string section = null)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section == null || section.ToString().Contains("appSettings"))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection("appSettings");
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
    }
}
