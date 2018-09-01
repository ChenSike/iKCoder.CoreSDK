using System;
using System.Collections.Generic;
using System.Text;

namespace iKCoderSDK.Util
{
    public class Util_SqlStringHelper
    {
        public static string SQL_GETALLTABLES_FOR_SQL2008 = "select * from sys.tables where (type = 'U')";
        public static string SQL_GETALLTABLES_FOR_SQL2005 = "select * from sys.all_objects where (type = 'U')";
        private static string SQL_GETALLTABLES_FOR_MYSQL = "select table_name from information_schema.tables where table_schema = '{schemaname}'";
        private static string SQL_GETALLSPS_FOR_MYSQL = "select `name` from MySQL.proc where db = '{schemaname}'";

        public static string Get_SQL_GETALLTABLES_FOR_MYSQL(string schemaname)
        {
            return SQL_GETALLTABLES_FOR_MYSQL.Replace("{schemaname}", schemaname);
        }

        public static string Get_SQL_GETALLSPS_FOR_MYSQL(string schemaname)
        {
            return SQL_GETALLSPS_FOR_MYSQL.Replace("{schemaname}", schemaname);
        }
    }
}
