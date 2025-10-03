using BaseClass.Base.Interface;
using BaseClass.Encryption.Interface;
using BaseClass.Model;
using BaseLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Encryption.Encryptions
{
    public class HybridEncryption : IEncryption
    {
        private readonly IBaseProvider? baseConfig;
        private ILogger? _logWriter;

        public bool IsDecrypted { get; set; }
        public bool IsEncrypted { get; set; }

        public HybridEncryption(IBaseProvider? BaseConfig, EncryptionModel? EncModel, ConfigAccessMode? AccessMode)
        {
            baseConfig = BaseConfig;
            _logWriter = BaseConfig?.GetItem<ILogger>();
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
            throw new NotImplementedException();
        }

        public string RegistryRead()
        {
            throw new NotImplementedException();
        }

        public void RegistrySave(string data)
        {
            throw new NotImplementedException();
        }
    }
}
