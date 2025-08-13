using BaseClass.Base.Interface;
using BaseClass.Encryption.Interface;
using BaseLogger;
using BaseLogger.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Encryption.Encryptions
{
    public class AESEncryption : IEncryption
    {
        private readonly IBase? baseConfig;
        private LogWriter? _logWriter;
        private string _filepath;
        private readonly byte[]? _Key;
        private readonly byte[]? _IV;
        //private ExeConfigurationFileMap _fileMap;
        private static bool keysGenerated;
        private static bool keysExist;
        private AesCng AESCng;

        public bool IsDecrypted { get; set; }
        public bool IsEncrypted { get; set; }

        public AESEncryption(IBase? BaseConfig) 
        {
            baseConfig = BaseConfig;
            _logWriter = BaseConfig?.Logger;

            AESCng = new AesCng();

            AESCng.Padding = PaddingMode.PKCS7;
            AESCng.Mode = CipherMode.CBC;        // Set padding mode
            AESCng.KeySize = 256;                // Set key size (128, 192, or 256 bits)
            AESCng.BlockSize = 128;              // AES block size is fixed at 128 bits

            //// Setting the constructor for the ExeConfig FilePath:
            //_fileMap = new ExeConfigurationFileMap
            //{
            //    ExeConfigFilename = baseConfig.ConfigPath,
            //};

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

        public void GenerateandSaveEncryptionKeys()
        {
            
        }

        public string RegistryRead()
        {
            throw new NotImplementedException();
        }

        public void RegistrySave(string data)
        {
            throw new NotImplementedException();
        }

        private bool PrivateKeyVerification()
        {
            try
            {
                string RegistyKeyName = RegistryRead();
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistyKeyName, false);
                object keyvalue1 = key.GetValue("KeyA");
                object keyvalue2 = key.GetValue("KeyIV");

                if (keyvalue1 is byte[] myByte1 && keyvalue2 is byte[] mybyte2)
                {
                    if (myByte1.Length == 0 && mybyte2.Length == 0)
                    {
                        keysGenerated = false;
                    }
                    else if ((myByte1.Length == 0 || mybyte2.Length != 0) && myByte1.Length != 0 && mybyte2.Length == 0)
                    {
                        keysGenerated = false;
                        _logWriter.LogWrite($"Keys did not generate correctly.", GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    }
                    else
                    {
                        keysGenerated = true;
                    }
                }
                else
                {
                    _logWriter.LogWrite($"Value/s does not exist", GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    keysGenerated = false;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Key verification had failed. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
            }

            return keysGenerated;
        }
    }
}
