using BaseLogger;
using CustomMessageBox.MVVM.Views.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Base.Interface
{
    public interface IBase
    {
        public LogWriter Logger { get; set; }
        public BaseMessageBox Messagebox { get; set; }
        public string? ConfigPath { get; set; }
        //public string? ConfigFileName { get; set; }
        public string? LoggedOnUser { get; set; }
        public string? LoggedOnUserGroup { get; set; }
    }
}
