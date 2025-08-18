using BaseClass.Base.Interface;
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
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Encryption.Encryptions
{
    public class AESEncryption : IEncryption
    {
        private readonly IBase? baseConfig;
        private readonly EncryptionModel? _encModel;
        private LogWriter? _logWriter;
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

        public AESEncryption(IBase? BaseConfig, EncryptionModel? EncModel, ConfigAccessMode? AccessMode) 
        {
            baseConfig = BaseConfig;
            _encModel = EncModel;
            _logWriter = BaseConfig?.Logger;

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
                _logWriter.LogWrite($"Key does not exist in the container. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
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
                        _regHandler?.RegistryValSave(data, data2);
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
                _logWriter.LogWrite($"Key was not generated and saved. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
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
                        _logWriter?.LogWrite($"Keys did not generate correctly.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    }
                    else
                    {
                        keysGenerated = true;
                    }
                }
                else
                {
                    _logWriter?.LogWrite($"Value/s does not exist", GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    keysGenerated = false;
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogWrite($"Key verification had failed. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
            }

            return keysGenerated;
        }

    }
}
