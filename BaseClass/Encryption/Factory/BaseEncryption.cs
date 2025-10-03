using BaseClass.Base.Interface;
using BaseClass.Database.Databases;
using BaseClass.Database.Interface;
using BaseClass.Encryption.Encryptions;
using BaseClass.Encryption.Interface;
using BaseClass.Model;
using BaseLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Encryption.Factory
{
    public static class BaseEncryption
    {
        public static IEncryption GetEncryption(EncryptionMode mode, ConfigAccessMode accessMode, EncryptionModel encModel, IBaseProvider provider)
        {
            return mode switch
            {
                EncryptionMode.AES => new AESEncryption(provider, encModel, accessMode),
                EncryptionMode.RSA => new RSAEncryption(provider, encModel, accessMode),
                EncryptionMode.Hybrid => new HybridEncryption(provider, encModel, accessMode),
                _ => throw new ArgumentException("Invalid mode", nameof(mode))
            };
        }
    }
}
