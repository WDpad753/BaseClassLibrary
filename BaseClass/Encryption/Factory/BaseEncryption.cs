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
        public static IEncryption GetEncryption(EncryptionMode mode, IBase settings)
        {
            return mode switch
            {
                EncryptionMode.AES => new AESEncryption(settings),
                EncryptionMode.RSA => new RSAEncryption(settings),
                EncryptionMode.Hybrid => new HybridEncryption(settings),
                _ => throw new ArgumentException("Invalid mode", nameof(mode))
            };
        }
    }
}
