using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.DataAccess
{
    public static class DataAccess
    {
        private static readonly SQLDataAccess connection = new SQLDataAccess();
        public static IDataBaseConnection Connection = connection;

        public static string GetConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}