﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLogger.Models
{
    //public enum MessageLevels
    //{
    //    Fatal = 0,
    //    //Error = 0,
    //    Main = 1,
    //    Trace = 2,
    //    Debug = 3,
    //    Info = 4
    //}
    
    public enum MessageLevels
    {
        Fatal = 0,
        //Error = 0,
        Log = 1,
        Verbose = 2,
        Debug = 3,
        Info = 4
    }
}
