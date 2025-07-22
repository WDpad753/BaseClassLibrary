using BaseClass.Base.Interface;
using BaseClass.Database.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Database.Databases
{
    public class SQLite : IDatabase
    {
        private readonly IBase baseClass;
        
        public SQLite(IBase BaseClass) 
        {
            baseClass = BaseClass;
        }
    }
}
