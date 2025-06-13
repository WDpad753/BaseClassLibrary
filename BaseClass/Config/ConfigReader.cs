using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Common.Abstractions.Models;
using Common.Abstractions;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Config
{
    public class ConfigReader : IConfigReader
    {
        private string configPath;
        private ExeConfigurationFileMap _fileMap;
        private string? _filepath;
        //private LogWriter _logWriter;
        private readonly ILogWriter _logWriter;
        //private connectionStringsSection _configConnSection;
        private AppSettingsSection _configAppSettingsSection;
        public bool _ConfigRead = false;
        private DebugState _debugState;

        public ConfigReader(string? filepath, DebugState state = 0)
        {
            _filepath = filepath;
            _debugState = state;

            //_logWriter = new LogWriter();
            _fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = filepath,
            };
        }

        public void SaveInfo(string data, string path, string? section)
        {
            try
            {
                Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                if (section != null && section.ToString().Contains("connectionStringsSection"))
                {
                    //_configConnSection = (connectionStringsSection)config.GetSection(section);
                }
                else if (section != null && section.ToString().Contains("appSettings"))
                {
                    _configAppSettingsSection = (AppSettingsSection)config.GetSection(section);
                }
                else
                {
                    _ConfigRead = false;
                    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
                }

                //if (_configConnSection != null && section.Contains("Conn", StringComparison.OrdinalIgnoreCase))
                //{
                //    _logWriter.LogWrite("There is/are connection strings in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Warning, _debugState);

                //    var keyval = _configConnSection.ConnectionStringItems.Cast<connectionStringSectionElement>().FirstOrDefault(e => e.Key == path);

                //    if (keyval != null)
                //    {
                //        keyval.Value = data;

                //        config.Save(ConfigurationSaveMode.Modified);

                //        // Refresh the section
                //        ConfigurationManager.RefreshSection(section);
                //    }
                //    else
                //    {
                //        _logWriter.LogWrite("Element does not exist in file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                //    }
                //}
                //else if (_configServSection != null && section.Contains("Serv", StringComparison.OrdinalIgnoreCase))
                //{
                //    _logWriter.LogWrite("There is/are Service Settings in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Warning, _debugState);

                //    var keyval = _configServSection.ServiceSettingsItems.Cast<serviceSectionElement>().FirstOrDefault(e => e.Key == path);

                //    if (keyval != null)
                //    {
                //        keyval.Value = data;

                //        config.Save(ConfigurationSaveMode.Modified);

                //        // Refresh the section
                //        ConfigurationManager.RefreshSection(section);
                //    }
                //    else
                //    {
                //        _logWriter.LogWrite("Element does not exist in file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                //    }
                //}
                //else
                //{
                //    _logWriter.LogWrite("There are no connection string/s in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Warning, _debugState);
                //}
                //return null;
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
            }
        }

        public string ReadInfo(string path, string? section)
        {
            try
            {
                Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);

                //if (section != null && section.ToString().Contains("connectionStringsSection"))
                //{
                //    _configConnSection = (connectionStringsSection)config.GetSection(section);
                //}
                //else if (section != null && section.ToString().Contains("serviceSection"))
                //{
                //    _configServSection = (serviceSection)config.GetSection(section);
                //}
                //else
                //{
                //    _logWriter.LogWrite("Unknown Config Section", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
                //}

                //if (_configConnSection != null)
                //{
                //    _logWriter.LogWrite("There is/are connection strings in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug, _debugState);

                //    var keyval = _configConnSection.ConnectionStringItems.Cast<connectionStringSectionElement>().FirstOrDefault(e => e.Key == path);

                //    if (keyval != null)
                //    {
                //        return keyval.Value.ToString();
                //    }
                //    else
                //    {
                //        _logWriter.LogWrite("Element does not exist in file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
                //        return null;
                //    }
                //}
                //else if (_configServSection != null)
                //{
                //    _logWriter.LogWrite("There is/are connection strings in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Debug, _debugState);

                //    var keyval = _configServSection.ServiceSettingsItems.Cast<serviceSectionElement>().FirstOrDefault(e => e.Key == path);

                //    if (keyval != null)
                //    {
                //        return keyval.Value.ToString();
                //    }
                //    else
                //    {
                //        _logWriter.LogWrite("Element does not exist in file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
                //        return null;
                //    }
                //}
                //else
                //{
                //    _logWriter.LogWrite("There are no connection string/s in the Configuration file.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Trace, _debugState);
                //    return null;
                //}
                return null;
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal, _debugState);
                return null;
            }
        }
    }
}
