using BaseClass.Base.Interface;
using BaseClass.Database.Databases;
using BaseClass.Database.Interface;
using BaseClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Database.Factory
{
    public static class DatabaseFactory
    {
        public static IDatabase GetDatabase(DatabaseMode mode, IBase settings)
        {
            return mode switch
            {
                DatabaseMode.SQLServer => new SQLServer(settings),
                DatabaseMode.SQLite => new SQLite(settings),
                //DatabaseMode.PostGresSQL => new PostGresSQL(settings),
                _ => throw new ArgumentException("Invalid mode", nameof(mode))
            };
        }
    }
}
