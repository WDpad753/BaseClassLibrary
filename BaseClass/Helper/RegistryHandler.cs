using BaseClass.Base.Interface;
using BaseClass.Config;
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
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class RegistryHandler
    {
        private readonly IBase? baseConfig;
        private readonly LogWriter? _logWriter;
        private readonly ConfigHandler? _configHandler;
        private readonly EnvHandler? _envHandler;
        private readonly EnvFileHandler? _envFileHandler;
        private ExeConfigurationFileMap? _fileMap;
        private readonly string? _configPath;

        public RegistryHandler(IBase? BaseConfig) 
        {
            baseConfig = BaseConfig;
            _logWriter = BaseConfig?.Logger;
            _configHandler = BaseConfig?.ConfigHandler;
            _envHandler = BaseConfig?.EnvHandler;

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

        public string? RegistryRead(object? PathKey, object? Key)
        {
            string? pathKey = null;
            string? key = null;
            string? configvalue = null;

            try
            {
                Configuration? config = null;

                switch (PathKey)
                {
                    case string s:
                        pathKey = s;
                        break;
                    case byte[] b:
                        byte[] decrypteddata = ProtectedData.Unprotect(b, null, DataProtectionScope.CurrentUser);
                        pathKey = Encoding.UTF8.GetString(decrypteddata);
                        break;
                }
                switch (Key)
                {
                    case string s:
                        key = s;
                        break;
                    case byte[] b:
                        byte[] decrypteddata = ProtectedData.Unprotect(b, null, DataProtectionScope.CurrentUser);
                        key = Encoding.UTF8.GetString(decrypteddata);
                        break;
                }

                if (_fileMap != null)
                {
                    config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);
                    configvalue = config.AppSettings.Settings[pathKey].Value;
                }
                else
                {
                    //configvalue = _envHandler?.EnvFileRead(_configPath, PathKey, Key);/**/
                    configvalue = _envHandler?.EnvRead(key, EnvAccessMode.File, _configPath, pathKey);

                    if (configvalue == null)
                    {
                        _logWriter?.LogWrite("Unable to find the Registry", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }

                if (configvalue != null)
                {
                    _logWriter?.LogWrite("There is Registry Path", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    _logWriter?.LogWrite($"RegistryKey Value = {configvalue}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    byte[] encrypteddata = Convert.FromBase64String(configvalue);
                    byte[] decryptedData = ProtectedData.Unprotect(encrypteddata, null, DataProtectionScope.CurrentUser);
                    string keyval = Encoding.UTF8.GetString(decryptedData);

                    if (keyval != null)
                    {
                        return keyval;
                    }
                    else
                    {
                        _logWriter?.LogWrite("Element does not exist in file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }
                else
                {
                    _logWriter?.LogWrite("There is no registry path in the Configuration file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
            finally
            {
                pathKey = null;
                key = null;
                configvalue = null;
            }
        }

        public void RegistrySave(object? PathKey, object? Key, string? data)
        {
            string? pathKey = null;
            string? key = null;
            string? configvalue = null;

            try
            {
                Configuration? config = null;

                switch (PathKey)
                {
                    case string s:
                        pathKey = s;
                        break;
                    case byte[] b:
                        byte[] decrypteddata = ProtectedData.Unprotect(b, null, DataProtectionScope.CurrentUser);
                        pathKey = Encoding.UTF8.GetString(decrypteddata);
                        break;
                }
                switch (Key)
                {
                    case string s:
                        key = s;
                        break;
                    case byte[] b:
                        byte[] decrypteddata = ProtectedData.Unprotect(b, null, DataProtectionScope.CurrentUser);
                        key = Encoding.UTF8.GetString(decrypteddata);
                        break;
                }

                if (_fileMap != null)
                {
                    config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);
                    configvalue = config.AppSettings.Settings[pathKey].Value;

                    if (configvalue == null)
                    {
                        _logWriter?.LogWrite("Unable to find the Registry", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return;
                    }
                }
                else
                {
                    //configvalue = _envHandler?.EnvFileRead(_configPath, PathKey, Key);
                    configvalue = _envHandler?.EnvRead(key, EnvAccessMode.File, _configPath, pathKey);

                    if (configvalue == null)
                    {
                        _logWriter?.LogWrite("Unable to find the Registry", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                        return;
                    }
                }

                if (configvalue != null || configvalue == "")
                {
                    _logWriter?.LogWrite("There is Registry Key Setting in Config File", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);

                    byte[] DataBytes = Encoding.UTF8.GetBytes(data);
                    byte[] EncryptedData = ProtectedData.Protect(DataBytes, null, DataProtectionScope.CurrentUser);
                    string encrypteddata = Convert.ToBase64String(EncryptedData);

                    if (encrypteddata != null)
                    {
                        if (_fileMap != null)
                        {
                            config.AppSettings.Settings["RegistryKey"].Value = encrypteddata;
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        else
                        {
                            //configvalue = _envHandler?.EnvFileSave(encrypteddata, PathKey, Key);    
                            _envHandler?.EnvSave(key, encrypteddata, EnvAccessMode.File, _configPath, pathKey);
                        }
                    }
                    else
                    {
                        _logWriter?.LogWrite("Element does not exist in file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    }
                }
                else
                {
                    _logWriter?.LogWrite("There is no registry path in the Configuration file.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogWrite($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
            }
        }
    }
}
