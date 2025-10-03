using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Base.Interface
{
    public interface IBaseSettings
    {
        public string? EncryptPathType { get; set; }
        public string? ConfigPath { get; set; }
        public string? LoggedOnUser { get; set; }
        public string? LoggedOnUserGroup { get; set; }
        public Uri? BaseUrlAddress { get; set; }
        public string? FilePath { get; set; }
        public string? DBServer { get; set; }
        public string? DBName { get; set; }
        public string? DBUser { get; set; }
        public string? DBPassword { get; set; }
        public string? DBPath { get; set; }
    }
}
