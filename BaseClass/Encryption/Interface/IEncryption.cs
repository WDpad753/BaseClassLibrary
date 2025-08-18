using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Encryption.Interface
{
    public interface IEncryption
    {
        bool IsDecrypted { get; set; }
        bool IsEncrypted { get; set; }
        string Encrypt(string data);
        string Decrypt(string data);
        void EnvSave(string data);
        string EnvRead();
        void GenerateandSaveEncryptionKeys();
    }
}
