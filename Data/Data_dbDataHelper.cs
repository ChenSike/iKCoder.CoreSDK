using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Xml;


namespace iKCoderSDK
{
    public class Data_dbDataHelper
    {
		

        public static bool GetColumnData(DataRow activeDR, string activeColumnName, out string result)
        {
            result = "";
            if (activeDR != null)
            {
                try
                {

                    result = activeDR[activeColumnName].ToString();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        public static bool GetArrByteColumnDataToString(DataRow activeDR, string activeColumnName, out string result)
        {
            result = "";
            if (activeDR != null)
            {
                try
                {
                    byte[] buffer = (byte[])activeDR[activeColumnName];
                    result = System.Text.Encoding.UTF8.GetString(buffer);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }


        public static byte[] GetArrBytesColumnData(DataRow activeDR, string activeColumnName)
        {
            if (activeDR != null)
            {
                try
                {
                    byte[] buffer = (byte[])activeDR[activeColumnName];
                    return buffer;
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }

        public static bool GetColumnsFromDT(DataTable activeDT, out List<string> result)
        {
            result = null;
            if (activeDT != null)
            {
                result = new List<string>();
                foreach (DataColumn actriveDC in activeDT.Columns)
                {
                    result.Add(actriveDC.ColumnName);
                }
                return true;
            }
            else
                return false;
        }

        public static bool GetColumnsTypeFromDT(DataTable activeDT, out Dictionary<string, Type> result)
        {
            result = null;
            if (activeDT != null)
            {
                result = new Dictionary<string, Type>();
                foreach (DataColumn actriveDC in activeDT.Columns)
                {
                    result.Add(actriveDC.ColumnName, actriveDC.DataType);
                }
                return true;
            }
            else
                return false;
        }

        public static bool GetActiveRow(DataTable activeDT, int activeRowIndex, out DataRow result)
        {
            result = null;
            if (activeDT != null)
            {
                try
                {
                    if (activeDT.Rows.Count >= activeRowIndex + 1)
                    {
                        result = activeDT.Rows[activeRowIndex];
                        return true;
                    }
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        public static bool ActionExecuteForDS(class_data_PlatformDBConnection activeconnection, string executeSql, out DataSet resultDS)
        {
            resultDS = null;
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand(executeSql);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand(executeSql);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteForDR(class_data_PlatformDBConnection activeconnection, string executeSql, out class_data_PlatformDBDataReader refDataReader)
        {
            refDataReader = null;
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        if (activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                        {
                            SqlCommand cmd = new SqlCommand(executeSql);
                            cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                            refDataReader = new class_data_PlatformDBDataReader();
                            refDataReader.activeDatabaseType = enum_DatabaseType.SqlServer;
                            ((class_data_SqlServerDataReader)refDataReader).ActiveDataReader = cmd.ExecuteReader();
                            return true;
                        }
                        else if (activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                        {
                            MySqlCommand cmd = new MySqlCommand(executeSql);
                            cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                            refDataReader = new class_data_PlatformDBDataReader();
                            refDataReader.activeDatabaseType = enum_DatabaseType.MySql;
                            ((class_data_MySqlDataReader)refDataReader).ActiveDataReader = cmd.ExecuteReader();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteStoreProcedureForDR(class_data_PlatformDBConnection activeconnection, class_Data_SqlSPEntry activeSPEntry, out class_data_PlatformDBDataReader refDataReader)
        {
            refDataReader = null;
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in ((class_data_SqlServerSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        refDataReader = new class_data_PlatformDBDataReader();
                        refDataReader.activeDatabaseType = enum_DatabaseType.SqlServer;
                        ((class_data_SqlServerDataReader)refDataReader).ActiveDataReader = cmd.ExecuteReader();
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (MySqlParameter activeParameter in ((class_data_MySqlSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        refDataReader = new class_data_PlatformDBDataReader();
                        refDataReader.activeDatabaseType = enum_DatabaseType.MySql;
                        ((class_data_MySqlDataReader)refDataReader).ActiveDataReader = cmd.ExecuteReader();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteStoreProcedureForDS(class_data_PlatformDBConnection activeconnection, class_Data_SqlSPEntry activeSPEntry, out DataSet resultDS)
        {
            resultDS = null;
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in ((class_data_SqlServerSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (MySqlParameter activeParameter in ((class_data_MySqlSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteStoreProcedureForDT(class_data_PlatformDBConnection activeconnection, class_Data_SqlSPEntry activeSPEntry, out DataTable resultDT)
        {
            resultDT = null;
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {

                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in ((class_data_SqlServerSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (MySqlParameter activeParameter in ((class_data_MySqlSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteSQLForDT(class_data_PlatformDBConnection activeconnection, string executeSql, out DataTable resultDT)
        {
            resultDT = null;
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand(executeSql);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand(executeSql);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
                    }
                    else
                        return false;

                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteSPForNonQuery(class_data_PlatformDBConnection activeconnection, class_Data_SqlSPEntry activeSPEntry)
        {
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in ((class_data_SqlServerSPEntry)activeSPEntry).GetActiveParametersList())
                            cmd.Parameters.Add(activeParameter);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
						try
						{
							MySqlCommand cmd = new MySqlCommand();
							cmd.CommandText = activeSPEntry.SPName;
							cmd.CommandType = CommandType.StoredProcedure;
							foreach (MySqlParameter activeParameter in ((class_data_MySqlSPEntry)activeSPEntry).GetActiveParametersList())
								cmd.Parameters.Add(activeParameter);
							cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
							MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
							cmd.ExecuteNonQuery();
							return true;
						}
						catch(Exception err)
						{
							return false;
						}
                    }
                    else
                        return false;

                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ActionExecuteForNonQuery(class_data_PlatformDBConnection activeconnection, string executeSql)
        {
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                    {
                        SqlCommand cmd = new SqlCommand(executeSql);
                        cmd.Connection = ((class_data_SqlServerConnectionItem)activeconnection).ActiveConnection;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    else if (activeconnection != null && activeconnection.activeDatabaseType == enum_DatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand(executeSql);
                        cmd.Connection = ((class_data_MySqlConnectionItem)activeconnection).ActiveConnection;
                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string ActionConvertDTtoXMLString(DataTable activeDataTable)
        {
            if (activeDataTable == null)
                return string.Empty;
            else
            {
                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml("<root></root>");
                XmlNode rootNode = resultDoc.SelectSingleNode("/root");
                int rowIndex = 1;
                foreach (DataRow dr in activeDataTable.Rows)
                {
                    XmlNode newRowNode = Util_XmlOperHelper.CreateNode(resultDoc, "row", "");
                    Util_XmlOperHelper.SetAttribute(newRowNode, "index", rowIndex.ToString());
                    foreach (DataColumn dc in activeDataTable.Columns)
                    {
                        string columnValue = "";
                        GetColumnData(dr, dc.ColumnName, out columnValue);
                        Util_XmlOperHelper.SetAttribute(newRowNode, dc.ColumnName, columnValue);
                    }
                    rootNode.AppendChild(newRowNode);
                }
                Util_XmlOperHelper.SetAttribute(rootNode, "itemcount", rowIndex.ToString());
                return resultDoc.OuterXml;
            }
        }
    }
}
