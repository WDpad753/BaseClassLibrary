﻿using BaseClass.Base.Interface;
using BaseClass.Database.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Database.Databases
{
    public class SQLServer : IDatabase
    {
        private readonly IBase baseClass;

        public SQLServer(IBase BaseClass)
        {
            baseClass = BaseClass;
        }
    }
}
