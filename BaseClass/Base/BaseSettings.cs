using BaseClass.Base.Interface;
using BaseClass.BaseRegistry;
using BaseClass.Config;
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
using static System.Net.Mime.MediaTypeNames;

namespace BaseClass.Base
{
    // For Base Classes
    public class BaseSettings : IBase
    {
        public ILogger? Logger { get; set; }
        public string? ConfigPath { get; set; }
        public string? LoggedOnUser { get; set; }
        public string? LoggedOnUserGroup { get; set; }
        public BaseMessageBox? Messagebox { get; set; }
        public BaseErrorMessageBox? ErrorMessagebox { get; set; }
        public Uri? BaseUrlAddress { get; set; }
        public string? FilePath { get; set; }
        public JSONFileHandler? JSONFileHandler { get; set; }
        public ConfigHandler? ConfigHandler { get; set; }
        public XmlHandler? XmlHandler { get; set; }
        public EnvFileHandler? EnvFileHandler { get; set; }
        public EnvHandler? EnvHandler { get; set; }
        public RegistryHandler? RegistryHandler { get; set; }
        public IDatabase? Database { get; set; }
        public IEncryption? Encryption { get; set; }
        public string? EncryptPathType { get; set; }
        public string? DBServer { get; set; }
        public string? DBName { get; set; }
        public string? DBUser { get; set; }
        public string? DBPassword { get; set; }
        public string? DBPath { get; set; }

        public BaseSettings()
        { }
    }
}
