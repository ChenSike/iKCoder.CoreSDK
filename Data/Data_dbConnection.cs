using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace iKCoderSDK
{

    public class class_Data_SqlConnectionItemEntry
    {
        public string Key
        {
            set;
            get;
        }

        public string Server
        {
            set;
            get;
        }

        public string UserID
        {
            set;
            get;
        }

        public string Password
        {
            set;
            get;
        }

        public string ActiveDataBase;
        public string ConnectionString = "server={server};uid={uid};pwd={pwd};database={db}";

        public class_data_PlatformDBConnection ActiveConnection
        {
            set;
            get;
        }

        public enum_DatabaseType ActiveConnectionType
        {
            set;
            get;
        }

    }

    public class class_Data_SqlConnectionHelper
    {
        public Dictionary<string, class_Data_SqlConnectionItemEntry> ActiveSqlConnectionCollection = new Dictionary<string, class_Data_SqlConnectionItemEntry>();

        public bool Set_NewConnectionItem(string Key, string Server, string UserID, string Password, string activeDB, enum_DatabaseType activeDBType)
        {
            if (!ActiveSqlConnectionCollection.ContainsKey(Key))
            {
                class_Data_SqlConnectionItemEntry newEntry = new class_Data_SqlConnectionItemEntry();
                newEntry.Server = Server;
                newEntry.UserID = UserID;
                newEntry.Password = Password;
                newEntry.ActiveDataBase = activeDB;
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{server}", Server);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{uid}", UserID);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{pwd}", Password);
                newEntry.ConnectionString = newEntry.ConnectionString.Replace("{db}", activeDB);
                newEntry.ActiveConnectionType = activeDBType;
                if (activeDBType == enum_DatabaseType.SqlServer)
                {
                    newEntry.ActiveConnection = new class_data_SqlServerConnectionItem();
                    newEntry.ActiveConnection.activeDatabaseType = activeDBType;
                    
                    ((class_data_SqlServerConnectionItem)newEntry.ActiveConnection).ActiveConnection = new SqlConnection(newEntry.ConnectionString);
                    try
                    {
                        ((class_data_SqlServerConnectionItem)newEntry.ActiveConnection).ActiveConnection.Open();
                        ActiveSqlConnectionCollection.Add(Key, newEntry);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else if (activeDBType == enum_DatabaseType.MySql)
                {
                    newEntry.ActiveConnection = new class_data_MySqlConnectionItem();
                    newEntry.ActiveConnection.activeDatabaseType = activeDBType;
                    ((class_data_MySqlConnectionItem)newEntry.ActiveConnection).ActiveConnection = new MySqlConnection(newEntry.ConnectionString);
                    try
                    {
                        ((class_data_MySqlConnectionItem)newEntry.ActiveConnection).ActiveConnection.Open();
                        ActiveSqlConnectionCollection.Add(Key, newEntry);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
                return false;
        }

        public class_data_PlatformDBConnection Get_ActiveConnection(string key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(key))
                return ActiveSqlConnectionCollection[key].ActiveConnection;
            else
                return null;
        }

        public class_data_MySqlConnectionItem Get_ActiveMySqlConnection(string key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(key))
            {
                if (ActiveSqlConnectionCollection[key].ActiveConnectionType == enum_DatabaseType.MySql)
                    return (class_data_MySqlConnectionItem)ActiveSqlConnectionCollection[key].ActiveConnection;
                else
                    return null;
            }
            else
                return null;
        }

        public class_data_SqlServerConnectionItem Get_ActiveSqlServerConnection(string key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(key))
            {
                if (ActiveSqlConnectionCollection[key].ActiveConnectionType == enum_DatabaseType.SqlServer)
                    return (class_data_SqlServerConnectionItem)ActiveSqlConnectionCollection[key].ActiveConnection;
                else
                    return null;
            }
            else
                return null;
        }

        public void Action_CloseAllActionConnection()
        {
            foreach (string key in ActiveSqlConnectionCollection.Keys)
            {
                try
                {
                    if (ActiveSqlConnectionCollection[key].ActiveConnectionType == enum_DatabaseType.MySql)
                        ((class_data_MySqlConnectionItem)ActiveSqlConnectionCollection[key].ActiveConnection).ActiveConnection.Close();
                    if (ActiveSqlConnectionCollection[key].ActiveConnectionType == enum_DatabaseType.SqlServer)
                        ((class_data_SqlServerConnectionItem)ActiveSqlConnectionCollection[key].ActiveConnection).ActiveConnection.Close();
                }
                catch
                {
                    continue;
                }
            }
            ActiveSqlConnectionCollection.Clear();
        }

        public bool Active_CloseActiveConnection(string Key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(Key))
            {

                if (ActiveSqlConnectionCollection[Key].ActiveConnectionType == enum_DatabaseType.MySql)
                    ((class_data_MySqlConnectionItem)ActiveSqlConnectionCollection[Key].ActiveConnection).ActiveConnection.Close();
                if (ActiveSqlConnectionCollection[Key].ActiveConnectionType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerConnectionItem)ActiveSqlConnectionCollection[Key].ActiveConnection).ActiveConnection.Close();

                ActiveSqlConnectionCollection.Remove(Key);
                return true;
            }
            else
                return false;
        }

        public bool Get_ExistedConnection(string Key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(Key))
                return true;
            else
                return false;
        }

        public bool Set_RemoveExistedConnection(string Key)
        {
            if (ActiveSqlConnectionCollection.ContainsKey(Key))
            {
                if (ActiveSqlConnectionCollection[Key].ActiveConnectionType == enum_DatabaseType.MySql)
                    ((class_data_MySqlConnectionItem)ActiveSqlConnectionCollection[Key].ActiveConnection).ActiveConnection.Close();
                if (ActiveSqlConnectionCollection[Key].ActiveConnectionType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerConnectionItem)ActiveSqlConnectionCollection[Key].ActiveConnection).ActiveConnection.Close();
                ActiveSqlConnectionCollection.Remove(Key);
                return true;
            }
            else
                return false;
        }

    }
}
