using BaseClass.API;
using BaseClass.Config;
using BaseClass.Database.Factory;
using BaseClass.Database.Interface;
using BaseClass.Encryption.Interface;
using BaseClass.Helper;
using BaseClass.JSON;
using BaseLogger;
using CustomMessageBox.MVVM.Views.ErrorMessageBox;
using CustomMessageBox.MVVM.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Base.Interface
{
    public interface IBase
    {
        public ILogger? Logger { get; set; }
        public BaseMessageBox? Messagebox { get; set; }
        public BaseErrorMessageBox? ErrorMessagebox { get; set; }
        public JSONFileHandler? JSONFileHandler { get; set; }
        public ConfigHandler? ConfigHandler { get; set; }
        public XmlHandler? XmlHandler { get; set; }
        public EnvFileHandler? EnvFileHandler { get; set; }
        public EnvHandler? EnvHandler { get; set; }
        public RegistryHandler? RegistryHandler { get; set; }
        public IDatabase? Database { get; set; }
        public IEncryption? Encryption { get; set; }
        public string? EncryptPathType { get; set; }
        public string? ConfigPath { get; set; }
        public string? LoggedOnUser { get; set; }
        public string? LoggedOnUserGroup { get; set; }
        public Uri? BaseUrlAddress { get; set; }
        public string? FilePath { get; set; }
    }
}
