using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql;
using MySql.Data.MySqlClient;

namespace iKCoderSDK
{
    public class Util_Data
    {
        public static DbType ConventStrTOCommonDbtye(string DBType)
        {
            switch (DBType)
            {
                case "text":
                    return DbType.String;
                case "date":
                    return DbType.Date;
                case "time":
                    return DbType.Time;
                case "smallint":
                    return DbType.Int16;
                case "int":
                    return DbType.Int32;
                case "real":
                case "decimal":
                    return DbType.Decimal;
                case "datetime":
                    return DbType.DateTime;
                case "float":
                    return DbType.Single;
                case "varchar":
                case "nvarchar":
                    return DbType.String;
                case "bit":
                    return DbType.Byte;
                case "binary":
                case "image":
                    return DbType.Binary;
                default:
                    return DbType.String;
            }
        }

        public static MySqlDbType ConventStrTOMySqlDbtye(string DBType)
        {
            string avtiveDBtype = DBType.ToLower();
            switch (DBType)
            {
                case "text":
                    return MySqlDbType.Text;
                case "tinytext":
                    return MySqlDbType.TinyText;
                case "longtext":
                    return MySqlDbType.LongText;
                case "mediumtext":
                    return MySqlDbType.MediumText;
                case "date":
                    return MySqlDbType.Date;
                case "datetime":
                    return MySqlDbType.DateTime;
                case "int":
                    return MySqlDbType.Int64;
                case "int32":
                    return MySqlDbType.Int32;
                case "double":
                    return MySqlDbType.Double;
                case "decimal":
                    return MySqlDbType.Decimal;
                case "float":
                    return MySqlDbType.Float;
                case "varchar":
                    return MySqlDbType.VarChar;
                case "blob":
                    return MySqlDbType.Blob;
                case "tinyblob":
                    return MySqlDbType.TinyBlob;
                case "mediumblob":
                    return MySqlDbType.MediumBlob;
                case "longblob":
                    return MySqlDbType.LongBlob;
                case "byte":
                    return MySqlDbType.Byte;
                case "binary":
                    return MySqlDbType.VarBinary;
                default:
                    return MySqlDbType.VarChar;
            }
        }

        public static SqlDbType ConventStrTODbtye(string DBType)
        {
            switch (DBType)
            {
                case "text":
                    return SqlDbType.Text;
                case "date":
                    return SqlDbType.Date;
                case "time":
                    return SqlDbType.Time;
                case "smallint":
                    return SqlDbType.Int;
                case "int":
                    return SqlDbType.BigInt;
                case "real":
                case "decimal":
                    return SqlDbType.Decimal;
                case "datetime":
                    return SqlDbType.DateTime;
                case "float":
                    return SqlDbType.Float;
                case "varchar":
                    return SqlDbType.VarChar;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "bit":
                    return SqlDbType.Bit;
                case "binary":
                    return SqlDbType.Binary;
                case "image":
                    return SqlDbType.Image;
                default:
                    return SqlDbType.NVarChar;
            }
        }
    }
}
