using BaseClass.Base.Interface;
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
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Config
{
    public class ConfigHandler
    {
        private readonly IBase? baseConfig;
        private ExeConfigurationFileMap? _fileMap;
        private EnvFileHandler? _envFileReader;
        private XmlHandler? _xmlHandler;
        private string? _filepath;
        private LogWriter? _logWriter;
        private AppSettingsSection? _configAppSettingsSection;
        private loggerSettings? _configLoggerSettingsSection;
        private changelogSettings? _configChangeLogSettingsSection;
        public bool _ConfigRead = false;
        private string? _targetSection;
        private static readonly Mutex ConfigFileMutex = new Mutex(false, "Global\\MyApp_ConfigFileMutex");

        public ConfigHandler(IBase baseSettings)
        {
            baseConfig = baseSettings;

            _logWriter = baseSettings.Logger;

            _fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = baseSettings.ConfigPath,
            };

            _envFileReader = new(baseSettings);
            //_xmlHandler = new(Logger, filepath);
            _xmlHandler = new(baseSettings);
            baseSettings.EnvFileHandler = _envFileReader;
            baseSettings.XmlHandler = _xmlHandler;
        }

        public void SaveInfo(string data, string path, string? section = null)
        {
            try
            {
                ConfigFileMutex.WaitOne(); // Wait for the mutex to be available

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section == null || section.ToString().Contains("appSettings") || section.ToString().Contains("appSettings".ToLower()))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection("appSettings");
                    _targetSection = "appSettings";
                }
                else if (section.ToString().Contains("loggerSettings") || section.ToString().Contains("loggerSettings".ToLower()))
                {
                    _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");
                    _targetSection = "loggerSettings";
                }
                else if (section.ToString().Contains("changelogSettings") || section.ToString().Contains("changelogSettings".ToLower()))
                {
                    _configChangeLogSettingsSection = (changelogSettings)config.GetSection("changelogSettings");
                    _targetSection = "changelogSettings";
                }
                else
                {
                    _ConfigRead = false;
                    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    return;
                }

                if (_configAppSettingsSection != null && _targetSection == "appSettings")
                {
                    _configAppSettingsSection.CurrentConfiguration.AppSettings.Settings[path].Value = data;

                    // Save the modified configuration
                    config.Save(ConfigurationSaveMode.Modified);

                    //Refresh the section
                    ConfigurationManager.RefreshSection("appSettings");

                    _logWriter.LogWrite($"{data} was saved in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                    _logWriter.LogWrite($"{data} was saved in {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                }
                else if (_configLoggerSettingsSection != null && _targetSection == "loggerSettings")
                {
                    //baseConfig.FilePath = _filepath == null ? _fileMap.ExeConfigFilename : _filepath;
                    _xmlHandler.XmlWrite(_targetSection, path, data);

                    // Mark the section as modified and save:
                    ConfigurationManager.RefreshSection("loggerSettings");

                    _logWriter.LogWrite($"{data} was saved in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                    _logWriter.LogWrite($"{data} was saved in {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                }
                else if (_configChangeLogSettingsSection != null && _targetSection == "changelogSettings")
                {
                    _xmlHandler.XmlWrite(_targetSection, path, data);

                    // Mark the section as modified and save:
                    ConfigurationManager.RefreshSection("changelogSettings");

                    _logWriter.LogWrite($"{data} was saved in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                    _logWriter.LogWrite($"{data} was saved in {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                }
                else
                {
                    _logWriter.LogWrite("Unable to save the data in the config file.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
            }
            finally
            {
                ConfigFileMutex.ReleaseMutex(); // Release the mutex
            }
        }

        public string? ReadInfo(string path, string? section = null)
        {
            try
            {
                ConfigFileMutex.WaitOne(); // Wait for the mutex to be available

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section == null || section.ToString().Contains("appSettings") || section.ToString().Contains("appSettings".ToLower()))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection("appSettings");
                    ConfigurationManager.RefreshSection("appSettings");
                    _targetSection = "appSettings";
                }
                else if (section.ToString().Contains("loggerSettings") || section.ToString().Contains("loggerSettings".ToLower()))
                {
                    _configLoggerSettingsSection = (loggerSettings)config.GetSection("loggerSettings");
                    ConfigurationManager.RefreshSection("loggerSettings");
                    _targetSection = "loggerSettings";
                }
                else if (section.ToString().Contains("changelogSettings") || section.ToString().Contains("changelogSettings".ToLower()))
                {
                    _configChangeLogSettingsSection = (changelogSettings)config.GetSection("changelogSettings");
                    ConfigurationManager.RefreshSection("changelogSettings");
                    _targetSection = "changelogSettings";
                }
                else
                {
                    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    return null;
                }

                if (_configAppSettingsSection != null && _targetSection == "appSettings")
                {
                    string? data = _configAppSettingsSection.CurrentConfiguration.AppSettings.Settings[path]?.Value;

                    _logWriter.LogWrite($"{data} was collected from {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    return data;
                }
                else if(_configLoggerSettingsSection != null && _targetSection == "loggerSettings")
                {
                    string? data = _configLoggerSettingsSection.LoggerSettings[path]?.value;

                    _logWriter.LogWrite($"{data} was collected from {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    return data;
                }
                else if(_configChangeLogSettingsSection != null && _targetSection == "changelogSettings")
                {
                    string? data = _configChangeLogSettingsSection.ChangeLogSettings[path]?.value;

                    _logWriter.LogWrite($"{data} was collected from {path} Key in Config File.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    return data;
                }
                else
                {
                    _logWriter.LogWrite("Unable to save the data in the config file.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
            finally
            {
                ConfigFileMutex.ReleaseMutex(); // Release the mutex
            }
        }
    }
}
