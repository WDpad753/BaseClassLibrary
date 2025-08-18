using BaseClass.Base.Interface;
using BaseLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseClass.Helper
{
    public static class DPAPIHandler
    {
        public static bool VerifyDPAPIData(byte[] encryptedBytes, byte[]? optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            try
            {
                ProtectedData.Unprotect(encryptedBytes, optionalEntropy, scope);
                
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        public static byte[]? Encrypt(string data, byte[]? optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            try
            {
                if (!StringHandler.IsValidBase64(data))
                    throw new Exception("Imported data is not valid Base 64.");

                byte[] bytes = Convert.FromBase64String(data);
                //byte[] bytes = Encoding.UTF8.GetBytes(data);
                return ProtectedData.Protect(bytes, optionalEntropy, scope);
            }
            catch(Exception ex)
            {
                throw new Exception($"Unable to encrypt the data. Exception: {ex}");
            }
        }

        public static string? Decrypt(byte[] encryptedBytes, byte[]? optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            try
            {
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, optionalEntropy, scope);
                //string val = Convert.ToBase64String(decryptedBytes);
                string val = Encoding.UTF8.GetString(decryptedBytes);
                return val;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to decrypt the data. Exception: {ex}");
            }
        }
    }
}
