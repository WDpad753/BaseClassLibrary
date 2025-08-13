using BaseClass.Base.Interface;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class RegistryHandler
    {
        private readonly IBase? baseConfig;
        private readonly LogWriter? _logWriter;
        private readonly EnvFileReader? _envHandler;
        private ExeConfigurationFileMap _fileMap;
        private readonly string? _configPath;

        public RegistryHandler(IBase? BaseConfig) 
        {
            baseConfig = BaseConfig;
            _logWriter = BaseConfig?.Logger;
            _envHandler = BaseConfig?.EnvFileReader;

            if (Path.GetExtension(baseConfig?.ConfigPath).ToString().Contains("config"))
            {
                // Setting the constructor for the ExeConfig FilePath:
                _fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = BaseConfig?.ConfigPath,
                };
            }
            else
            {
                _configPath = BaseConfig?.ConfigPath;
            }
        }

        public string? RegistryRead(string? PathKey, string? Key, RegPath? regPath)
        {
            try
            {
                string? configvalue = null;
                Configuration? config = null;

                if (_fileMap != null)
                {
                    config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);
                    configvalue = config.AppSettings.Settings[PathKey].Value;
                }
                else
                {
                    configvalue = _envHandler.EnvFileRead(_configPath, PathKey, Key);
                    if(configvalue == null)
                    {
                        _logWriter.LogWrite("Unable to find the Registry", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }

                if (configvalue != null)
                {
                    _logWriter.LogWrite("There is Registry Path", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    _logWriter.LogWrite($"RegistryKey Value = {configvalue}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    byte[] encrypteddata = Convert.FromBase64String(configvalue);
                    byte[] decryptedData = ProtectedData.Unprotect(encrypteddata, null, DataProtectionScope.CurrentUser);
                    string keyval = Encoding.UTF8.GetString(decryptedData);

                    if (keyval != null)
                    {
                        return keyval;
                    }
                    else
                    {
                        _logWriter.LogWrite("Element does not exist in file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }
                else
                {
                    _logWriter.LogWrite("There is no registry path in the Configuration file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }
    }
}
