using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;

namespace iKCoderSDK
{
    public class Data_dbBase
    {
        public enum enum_DatabaseType
        {
            SqlServer = 1,
            MySql = 2
        }

        public class class_data_PlatformDBConnection
        {
            public enum_DatabaseType activeDatabaseType
            {
                set;
                get;
            }
        }

        public class class_data_PlatformDBDataReader
        {
            public enum_DatabaseType activeDatabaseType
            {
                set;
                get;
            }
        }

        public class class_data_PlatformDBCommand
        {
            public enum_DatabaseType activeDatabaseType
            {
                set;
                get;
            }
        }

        public class class_data_MySqlCommand : class_data_PlatformDBCommand
        {
            public MySqlCommand ActiveCommand
            {
                set;
                get;
            }
        }

        public class class_data_SqlServerCommand : class_data_PlatformDBCommand
        {
            
            public SqlCommand ActiveCommand
            {
                set;
                get;
            }
        }

        public class class_data_SqlServerDataReader : class_data_PlatformDBDataReader
        {
            public SqlDataReader ActiveDataReader
            {
                set;
                get;
            }
        }

        public class class_data_MySqlDataReader : class_data_PlatformDBDataReader
        {
            public MySqlDataReader ActiveDataReader
            {
                set;
                get;
            }
        }

        public class class_data_SqlServerConnectionItem : class_data_PlatformDBConnection
        {
            public SqlConnection ActiveConnection
            {
                set;
                get;
            }
        }

        public class class_data_MySqlConnectionItem : class_data_PlatformDBConnection
        {
            public MySqlConnection ActiveConnection
            {
                set;
                get;
            }
        }

        [Serializable()]
        public class class_Data_SqlSPEntry
        {
            public class_Data_SqlSPEntry(enum_DatabaseType activeDBType)
            {
                this.ActiveDBType = activeDBType;
            }

            public enum_DatabaseType ActiveDBType
            {
                set;
                get;
            }

            public string SPName
            {
                set;
                get;
            }

            public string KeyName
            {
                set;
                get;
            }

            public string EntryType
            {
                set;
                get;
            }

            public virtual object Clone()
            {
                object newEntry = (new class_Data_SqlSPEntry(this.ActiveDBType)) as object;
                ((class_Data_SqlSPEntry)newEntry).EntryType = this.EntryType;
                ((class_Data_SqlSPEntry)newEntry).KeyName = this.KeyName;
                ((class_Data_SqlSPEntry)newEntry).SPName = this.SPName;
                return newEntry;
            }

            public virtual bool ModifyParameterSize(string Paraname, int SPSize)
            {
                return true;
            }

            public virtual bool ModifyParameterValue(string Paraname, object SPValue)
            {
                return true;
            }

            public virtual bool ModifyParameterValue(string Paraname, object SPValue, int Size)
            {
                return true;
            }

            public virtual bool ModifyParameterDirection(string Paraname, ParameterDirection SPDirection)
            {
                return true;
            }

            public virtual void ClearAllParams()
            {
            }

            public virtual void ClearAllParamsValues()
            {
            }

        }
    }

    public enum enum_DatabaseType
    {
        SqlServer = 1,
        MySql = 2
    }

    public class class_data_PlatformDBConnection
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }
    }

    public class class_data_PlatformDBDataReader
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }
    }

    public class class_data_PlatformDBCommand
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }
    }

    public class class_data_MySqlCommand : class_data_PlatformDBCommand
    {
        public MySqlCommand ActiveCommand
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerCommand : class_data_PlatformDBCommand
    {
        public DbCommand ActiveCommand
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerDataReader : class_data_PlatformDBDataReader
    {
        public DbDataReader ActiveDataReader
        {
            set;
            get;
        }
    }

    public class class_data_MySqlDataReader : class_data_PlatformDBDataReader
    {
        public MySqlDataReader ActiveDataReader
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerConnectionItem : class_data_PlatformDBConnection
    {
        public SqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

    public class class_data_MySqlConnectionItem : class_data_PlatformDBConnection
    {
        public MySqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

    [Serializable()]
    public class class_Data_SqlSPEntry
    {
        public class_Data_SqlSPEntry(enum_DatabaseType activeDBType)
        {
            this.ActiveDBType = activeDBType;
        }

        public enum_DatabaseType ActiveDBType
        {
            set;
            get;
        }

        public string SPName
        {
            set;
            get;
        }

        public string KeyName
        {
            set;
            get;
        }

        public string EntryType
        {
            set;
            get;
        }

        public virtual object Clone()
        {
            object newEntry = (new class_Data_SqlSPEntry(this.ActiveDBType)) as object;
            ((class_Data_SqlSPEntry)newEntry).EntryType = this.EntryType;
            ((class_Data_SqlSPEntry)newEntry).KeyName = this.KeyName;
            ((class_Data_SqlSPEntry)newEntry).SPName = this.SPName;
            return newEntry;
        }

        public virtual bool ModifyParameterSize(string Paraname, int SPSize)
        {
            return true;
        }

        public virtual bool ModifyParameterValue(string Paraname, object SPValue)
        {
            return true;
        }

        public virtual bool ModifyParameterValue(string Paraname, object SPValue, int Size)
        {
            return true;
        }

        public virtual bool ModifyParameterDirection(string Paraname, ParameterDirection SPDirection)
        {
            return true;
        }

        public virtual void ClearAllParams()
        {
        }

        public virtual void ClearAllParamsValues()
        {
        }

    }

}
