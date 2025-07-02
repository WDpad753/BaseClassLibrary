using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public static class Crc32
    {
        private static readonly uint[] Table;

        static Crc32()
        {
            Table = new uint[256];
            const uint polynomial = 0xEDB88320;
            for (uint i = 0; i < Table.Length; ++i)
            {
                uint crc = i;
                for (int j = 0; j < 8; ++j)
                    crc = (crc >> 1) ^ ((crc & 1) != 0 ? polynomial : 0);
                Table[i] = crc;
            }
        }

        public static string CalculateHash<T>(T input)
        {
            // Convert the object back to normalized JSON string
            var jsonString = JsonSerializer.Serialize(input, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            // Compute SHA256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                    hashBuilder.Append(b.ToString("x2"));
                return hashBuilder.ToString();
            }
        }

        public static uint ComputeFromDigits(string input)
        {
            // Extract digits only
            string digitsOnly = Regex.Replace(input, @"\D", "");
            byte[] bytes = Encoding.ASCII.GetBytes(digitsOnly);

            uint crc = 0xFFFFFFFF;
            foreach (byte b in bytes)
                crc = (crc >> 8) ^ Table[(crc ^ b) & 0xFF];

            return ~crc;
        }
    }
}
