using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Model
{
    public class EncryptionModel
    {
        public byte[]? PathKey { get; set; }
        public byte[]? Key { get; set; }
        public byte[]? RegType { get; set; }
        public byte[]? EnvType { get; set; }
        public List<byte[]>? Keys { get; set; }
    }
}
