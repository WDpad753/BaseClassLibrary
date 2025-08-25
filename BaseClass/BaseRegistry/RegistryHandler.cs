using BaseClass.Base.Interface;
using BaseClass.Config;
using BaseClass.Helper;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace BaseClass.BaseRegistry
{
    public class RegistryHandler
    {
        private readonly IBase? baseConfig;
        private readonly ILogger? _logWriter;
        private readonly ConfigHandler? _configHandler;
        private readonly EnvHandler? _envHandler;
        private readonly EnvFileHandler? _envFileHandler;
        private ExeConfigurationFileMap? _fileMap;
        private readonly string? _configPath;
        private readonly EncryptionModel? _encModel;

        public RegistryHandler(IBase? BaseConfig, EncryptionModel? EncModel)
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

            _encModel=EncModel;
        }

        public string? RegistryRead(string? PathKey, object? Key = null)
        {
            string? pathKey = null;
            string? key = null;
            string? configvalue = null;

            try
            {
                Configuration? config = null;

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
                    configvalue = config.AppSettings.Settings[PathKey].Value;
                }
                else
                {
                    //configvalue = _envHandler?.EnvFileRead(_configPath, PathKey, Key);/**/
                    configvalue = _envHandler?.EnvRead(key, EnvAccessMode.File, _configPath, PathKey);

                    if (configvalue == null)
                    {
                        _logWriter?.LogError("Unable to find the Registry");
                        return null;
                    }
                }

                if (configvalue != null)
                {
                    _logWriter?.LogBase("There is Registry Path");
                    _logWriter?.LogDebug($"RegistryKey Value = {configvalue}");

                    string? keyval = null;
                    bool keyValVer = StringHandler.IsValidBase64(configvalue, true);

                    if(keyValVer == true)
                    {
                        keyval = DPAPIHandler.Decrypt(Convert.FromBase64String(configvalue));

                        if(StringHandler.IsValidBase64(keyval, true))
                        {
                            keyval = DPAPIHandler.Decrypt(Convert.FromBase64String(keyval));
                        }
                    }
                    else if(keyValVer == false)
                    {
                        keyval = configvalue;
                    }
                    else
                    {
                        throw new Exception("Inserted value of unknown type.");
                    }

                    if (keyval != null)
                    {
                        return keyval;
                    }
                    else
                    {
                        _logWriter?.LogError("Element does not exist in file.");
                        return null;
                    }
                }
                else
                {
                    _logWriter?.LogAlert("There is no registry path in the Configuration file.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
                return null;
            }
            finally
            {
                pathKey = null;
                key = null;
                configvalue = null;
            }
        }

        public void RegistrySave(string? PathKey, object? data, object? Key = null)
        {
            string? pathKey = null;
            string? key = null;
            string? Data = null;
            string? encrypteddata = null;
            string? configvalue = null;

            try
            {
                Configuration? config = null;

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
                switch (data)
                {
                    case string s:
                        byte[] DataBytes = Encoding.UTF8.GetBytes(s);
                        byte[] EncryptedData = ProtectedData.Protect(DataBytes, null, DataProtectionScope.CurrentUser);
                        encrypteddata = Convert.ToBase64String(EncryptedData);
                        break;
                    case byte[] b:
                        byte[] encryptedBytes = ProtectedData.Protect(b, null, DataProtectionScope.CurrentUser);
                        encrypteddata = Convert.ToBase64String(encryptedBytes);
                        break;
                }

                string? keyval = null;
                bool keyValVer = StringHandler.IsValidBase64(encrypteddata, true);

                if (keyValVer == true)
                {
                    if (encrypteddata != null)
                    {
                        if (_fileMap != null)
                        {
                            config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);
                            config.AppSettings.Settings[PathKey].Value = encrypteddata;
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        else
                        {
                            //configvalue = _envHandler?.EnvFileSave(encrypteddata, PathKey, Key);    
                            _envHandler?.EnvSave(key, encrypteddata, EnvAccessMode.File, _configPath, PathKey);
                        }
                    }
                    else
                    {
                        _logWriter?.LogError("Element does not exist in file.");
                    }
                }
                else if (keyValVer == false)
                {
                    if (_fileMap != null)
                    {
                        config = ConfigurationManager.OpenMappedExeConfiguration(_fileMap, ConfigurationUserLevel.None);
                        config.AppSettings.Settings[PathKey].Value = encrypteddata;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    else
                    {
                        //configvalue = _envHandler?.EnvFileSave(encrypteddata, PathKey, Key);    
                        _envHandler?.EnvSave(key, configvalue, EnvAccessMode.File, _configPath, PathKey);
                    }
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Path does not exist. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
            }
            finally
            {
                pathKey = null;
                key = null;
                configvalue = null;
            }
        }

        public void RegistryValSave(List<object>? datas, RegistryValueKind valueKind)
        {
            RegistryKey? key = null;

            try
            {
                string RegistyKeyName = RegistryRead(_encModel?.ConfigKey, _encModel?.Key);

                _logWriter?.LogDebug($"Actual Registry Path Value => {RegistryRead(_encModel?.ConfigKey)}");

                foreach (var Key in _encModel?.Keys)
                {
                    if (Encoding.UTF8.GetString(ProtectedData.Unprotect(_encModel?.RegType, null, DataProtectionScope.CurrentUser)).Equals(RegPath.User.ToString()))
                    {
                        Debug.WriteLine($"[{RegistryRead(_encModel?.ConfigKey)}]");
                        key = Registry.CurrentUser.OpenSubKey(RegistryRead(_encModel?.ConfigKey), true);
                    }
                    else if (Encoding.UTF8.GetString(ProtectedData.Unprotect(_encModel?.RegType, null, DataProtectionScope.CurrentUser)).Equals(RegPath.Machine.ToString()))
                    {
                        key = Registry.LocalMachine.OpenSubKey(RegistryRead(_encModel?.ConfigKey), true);
                    }
                    else
                    {
                        throw new Exception("Unknown Registry Type.");
                    }

                    byte[]? val = null;

                    Debug.WriteLine($"[{ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)}]");
                    Debug.WriteLine($"[{Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser))}]");
                    Debug.WriteLine($"[{key.GetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)))}]");

                    foreach(var Data in datas)
                    {
                        var value = RegistryValueHandler.RegistrySetValue(Data, valueKind);
                        key.SetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)), value, valueKind);
                    }
                }

                key.Close();
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Key was not saved. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
            }
        }

        public List<byte[]> RegistryValGet()
        {
            RegistryKey? key = null;

            try
            {
                List<byte[]> list = new List<byte[]>();

                _logWriter?.LogDebug($"Actual Registry Path Value => {RegistryRead(_encModel?.ConfigKey)}");

                foreach(var Key in _encModel?.Keys)
                {
                    if (Encoding.UTF8.GetString(ProtectedData.Unprotect(_encModel?.RegType, null, DataProtectionScope.CurrentUser)).Equals(RegPath.User.ToString()))
                    {
                        //key = Registry.CurrentUser.OpenSubKey(RegistyKeyName, true);
                        //var da = Encoding.UTF8.GetString(ProtectedData.Unprotect(Encoding.UTF8.GetBytes(RegistyKeyName), null, DataProtectionScope.CurrentUser));
                        Debug.WriteLine($"[{RegistryRead(_encModel?.ConfigKey)}]");
                        key = Registry.CurrentUser.OpenSubKey(RegistryRead(_encModel?.ConfigKey), false);
                    }
                    else if (Encoding.UTF8.GetString(ProtectedData.Unprotect(_encModel?.RegType, null, DataProtectionScope.CurrentUser)).Equals(RegPath.Machine.ToString()))
                    {
                        key = Registry.LocalMachine.OpenSubKey(RegistryRead(_encModel?.ConfigKey), false);
                    }
                    else
                    {
                        throw new Exception("Unknown Registry Type.");
                    }

                    byte[]? val = null;

                    Debug.WriteLine($"[{ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)}]");
                    Debug.WriteLine($"[{Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser))}]");
                    Debug.WriteLine($"[{key.GetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)))}]");
                    
                    val = ProtectedData.Protect(Encoding.UTF8.GetBytes((string)key.GetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(Key, null, DataProtectionScope.CurrentUser)))),null, DataProtectionScope.CurrentUser);

                    list.Add(val);
                }

                key.Close();

                return list;
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Key was not saved. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
                return null;
            }
        }
    }
}
