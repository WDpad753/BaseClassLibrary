﻿using BaseClass.Helper;
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
        //private string? configPath;
        private ExeConfigurationFileMap _fileMap;
        private EnvFileReader _envFileReader;
        private XmlHandler _xmlHandler;
        private string? _filepath;
        //private string? _targetSection;
        //private ILogWriter _logWriter;
        private LogWriter _logWriter;
        private AppSettingsSection? _configAppSettingsSection;
        private loggerSettings? _configLoggerSettingsSection;
        private changelogSettings? _configChangeLogSettingsSection;
        public bool _ConfigRead = false;
        private string? _targetSection;
        private static readonly Mutex ConfigFileMutex = new Mutex(false, "Global\\MyApp_ConfigFileMutex");
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
            _xmlHandler = new(Logger, filepath);
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

        public string? EnvRead(string path, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            string? data = null;

            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    _logWriter.LogWrite($"Was not able to obtain value from given path.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    _logWriter.LogWrite($"Was not able to obtain value from given path. Submitted path => {path}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
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
                    _logWriter.LogWrite($"Unable to obtain value from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);

                    return null;
                }
                else
                {
                    _logWriter.LogWrite($"Obtained following value {data} from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        public void EnvSave(string path, string? data = null, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(data))
                {
                    _logWriter.LogWrite($"Was not able to obtain value from given path.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    _logWriter.LogWrite($"Was not able to obtain value from given path. Submitted path => {path}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    return;
                }

                if (mode == null || mode == EnvAccessMode.Project)
                {
                    // Read environment variable from the current process scope
                    Environment.SetEnvironmentVariable(path, data);
                }
                else if (mode == EnvAccessMode.File)
                {
                    _envFileReader.EnvFileSave(envpath, path, envkeyname, data);
                }
                else if (mode == EnvAccessMode.User)
                {
                    Environment.SetEnvironmentVariable(path, data, EnvironmentVariableTarget.User);
                }
                else if (mode == EnvAccessMode.System)
                {
                    Environment.SetEnvironmentVariable(path, data, EnvironmentVariableTarget.Machine);
                }

                _logWriter.LogWrite($"Saved following value {data} from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }
    }
}
