﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Model
{
    public class CLGConfig
    {
        public string? commitmessagespath;
        public string? commitmessagesfilename;
        public string? commitlogpath = null;
        public string? commitlogfilename;
        public string? jsonpath;
        public string? backupjsonpath;
        public string? jsonfilename;
        public static string? aconfigpath;
        public static string? configfilename;
        public string? prevMapJsonHS;

        //public string filepath
        //{
        //    get
        //    {
        //        string commitmessages = Path.Combine(commitmessagespath, commitmessagesfilename);
        //        return commitmessages;
        //    }
        //}

        public string? logfilepath
        {
            get
            {
                string? commitmessages = Path.Combine(commitlogpath, commitlogfilename);
                return commitmessages;
            }
        }
    }
}
