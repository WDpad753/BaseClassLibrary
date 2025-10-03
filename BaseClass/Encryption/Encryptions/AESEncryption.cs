using BaseClass.Base.Interface;
using BaseClass.RegistryBase;
using BaseClass.Encryption.Interface;
using BaseClass.Helper;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaseClass.Encryption.Encryptions
{
    public class AESEncryption : IEncryption
    {
        private readonly IBaseProvider? baseConfig;
        private readonly EncryptionModel? _encModel;
        private ILogger? _logger;
        private RegistryHandler? _regHandler;
        private EnvHandler? _envHandler;
        private EnvFileHandler? _envFileHandler;
        //private string _filepath;
        private byte[]? _Key;
        private byte[]? _IV;
        //private ExeConfigurationFileMap _fileMap;
        private static bool keysGenerated;
        private static bool keysExist;
        private AesCng? AESCng;

        public bool IsDecrypted { get; set; }
        public bool IsEncrypted { get; set; }

        public AESEncryption(IBaseProvider? BaseConfig, EncryptionModel? EncModel, ConfigAccessMode? AccessMode) 
        {
            baseConfig = BaseConfig;
            _encModel = EncModel;
            _logger = BaseConfig?.GetItem<ILogger>();

            AESCng = new AesCng();

            AESCng.Padding = PaddingMode.PKCS7;
            AESCng.Mode = CipherMode.CBC;        // Set padding mode
            AESCng.KeySize = 256;                // Set key size (128, 192, or 256 bits)
            AESCng.BlockSize = 128;              // AES block size is fixed at 128 bits

            if (AccessMode.HasValue && AccessMode.Value == ConfigAccessMode.Registry)
            {
                _regHandler = new(BaseConfig,EncModel);
            }
            else if (AccessMode.HasValue && AccessMode.Value == ConfigAccessMode.Environment)
            {
                _envHandler = new(BaseConfig);
            }
            //else if (AccessMode.HasValue && (AccessMode.Value == ConfigAccessMode.EnvironmentFile || AccessMode.Value == ConfigAccessMode.JSONFile))
            //{
            //    _envFileHandler = new(BaseConfig);
            //}
            else
            {
                throw new Exception("Unable to find assigned access mode.");
            }
            
            GenerateandSaveEncryptionKeys();
        }

        public string Decrypt(string data)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string data)
        {
            throw new NotImplementedException();
        }

        public string EnvRead()
        {
            throw new NotImplementedException();
        }

        public void EnvSave(string data)
        {
            throw new NotImplementedException();
        }

        private void LoadPrivateKey()
        {
            try
            {
                List<byte[]>? data = _regHandler?.RegistryValGet();
                byte[] decrypteddata = ProtectedData.Unprotect(data[0], null, DataProtectionScope.CurrentUser);
                byte[] decrypteddata2 = ProtectedData.Unprotect(data[1], null, DataProtectionScope.CurrentUser);
                string KeyString = Encoding.UTF8.GetString(decrypteddata);
                string IVString = Encoding.UTF8.GetString(decrypteddata2);

                AESCng.Key = Convert.FromBase64String(KeyString);
                AESCng.IV = Convert.FromBase64String(IVString);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Key does not exist in the container. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
            }
        }

        public void GenerateandSaveEncryptionKeys()
        {
            try
            {
                bool? keyVer = null;

                keyVer = PrivateKeyVerification();

                if (keyVer != null)
                {
                    if ((bool)!keyVer)
                    {
                        AESCng.GenerateKey();
                        AESCng.GenerateIV();

                        _Key = AESCng.Key;
                        _IV = AESCng.IV;

                        var KeyString = Convert.ToBase64String(_Key);
                        var IVString = Convert.ToBase64String(_IV);
                        byte[] data = ProtectedData.Protect(Encoding.UTF8.GetBytes(KeyString), null, DataProtectionScope.CurrentUser);
                        byte[] data2 = ProtectedData.Protect(Encoding.UTF8.GetBytes(IVString), null, DataProtectionScope.CurrentUser);
                        List<object> Datas = new List<object>() { data, data2 };
                        _regHandler?.RegistryValSave(Datas, RegistryValueKind.Binary);
                    }
                    else
                    {
                        LoadPrivateKey();
                    }
                }
                else
                {
                    throw new Exception($"Key was not generated and saved.");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Key was not generated and saved. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
            }
        }

        private bool? PrivateKeyVerification()
        {
            object? keyvalue1 = null;
            object? keyvalue2 = null;
            RegistryKey? key = null;
            byte[]? RegType = null;

            try
            {
                if (_regHandler != null)
                {
                    string? RegistyKeyName = _regHandler.RegistryRead(_encModel?.ConfigKey, _encModel?.Key);

                    List<byte[]>? keys = _encModel?.Keys;

                    if(keys.Count > 1)
                    {
                        keyvalue1 = key.GetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(keys[0], null, DataProtectionScope.CurrentUser)));
                        keyvalue2 = key.GetValue(Encoding.UTF8.GetString(ProtectedData.Unprotect(keys[1], null, DataProtectionScope.CurrentUser)));
                    }
                    else
                    {
                        throw new InvalidOperationException("There has to be more than one key");
                    }
                }
                else if(_envHandler != null)
                {

                }
                else
                {
                    throw new Exception("Unable to verfiy the keys.");
                }


                if (keyvalue1 is byte[] myByte1 && keyvalue2 is byte[] mybyte2)
                {
                    if (myByte1.Length == 0 && mybyte2.Length == 0)
                    {
                        keysGenerated = false;
                    }
                    else if ((myByte1.Length == 0 || mybyte2.Length != 0) && myByte1.Length != 0 && mybyte2.Length == 0)
                    {
                        keysGenerated = false;
                        _logger?.LogDebug($"Keys did not generate correctly.");
                    }
                    else
                    {
                        keysGenerated = true;
                    }
                }
                else
                {
                    _logger?.LogAlert("Value/s does not exist");
                    keysGenerated = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Key verification had failed. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
            }

            return keysGenerated;
        }

    }
}
