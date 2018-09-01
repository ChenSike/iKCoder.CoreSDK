using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace iKCoderSDK
{

    public class class_Data_SqlStringHelper
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

    public class Data_dbOperations
    {

        public bool ActionExportsTablesAndDatasToXML(class_data_PlatformDBConnection ActiveConnection, string exportDir)
        {
            /*try
            {
                if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                {
                    string sql_getALLTables = class_Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;
                }
                else if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.MySql)
                {
                    class_data_MySqlConnectionItem mysqlActiveConnectionItem = (class_data_MySqlConnectionItem)ActiveConnection;
                    string sql_getALLTables = class_Data_SqlStringHelper.Get_SQL_GETALLTABLES_FOR_MYSQL(mysqlActiveConnectionItem.ActiveConnection.Database);
                    DataTable TableInfo = new DataTable();
                    StringBuilder sql_CreateNewSp = new StringBuilder();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TableInfo))
                    {
                        foreach(DataRow activeDataRow in TableInfo.Rows)
                        {
                            string tableName = Data_dbDataHelper.GetColumnData(activeDataRow, "table_name", out tableName);

                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }*/
            return true;
        }

        public bool ActionAutoCreateSPS(class_data_PlatformDBConnection ActiveConnection)
        {
            try
            {
                if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                {
                    string sql_getALLTables = class_Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;
                    DataTable TablesInfo = new DataTable();
                    DataTable ColumnInfo = new DataTable();
                    DataTable TypesInfo = new DataTable();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo))
                    {
                        if (TablesInfo.Rows.Count > 0)
                        {
                            foreach (DataRow activeDR_1 in TablesInfo.Rows)
                            {
                                string sql_getALLColumns = "select * from sys.syscolumns";
                                string tableName = "";
                                Data_dbDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                                string objectID = "";
                                Data_dbDataHelper.GetColumnData(activeDR_1, "object_id", out objectID);
                                if (tableName != "")
                                {
                                    if (objectID != "")
                                    {
                                        sql_getALLColumns = sql_getALLColumns + " where id='" + objectID + "'";
                                        List<string> activeColumn = new List<string>();
                                        List<string> activeKeyColumn = new List<string>();
                                        List<string> filterColumn = new List<string>();
                                        List<string> filterTypeList = new List<string>();
                                        if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLColumns, out ColumnInfo))
                                        {
                                            StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'spa_peration_" + tableName + "'), N'IsProcedure') = 1");
                                            sql_CreateNewSp.AppendLine();
                                            sql_CreateNewSp.AppendLine("DROP PROCEDURE spa_operation_" + tableName);
                                            Data_dbDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());
                                            sql_CreateNewSp.Clear();
                                            sql_CreateNewSp.AppendLine("CREATE PROCEDURE {SPNAME}");
                                            StringBuilder sql_insertSourceColumns = new StringBuilder();
                                            StringBuilder sql_insertValueColumns = new StringBuilder();
                                            sql_CreateNewSp.Replace("{SPNAME}", "spa_operation_" + tableName);
                                            sql_CreateNewSp.AppendLine("(");
                                            sql_CreateNewSp.AppendLine("@operation nvarchar(20) = '',");
                                            foreach (DataRow activeDR_2 in ColumnInfo.Rows)
                                            {
                                                string sql_getALLTypes = "select * from sys.types";
                                                string columnname = "";
                                                Data_dbDataHelper.GetColumnData(activeDR_2, "name", out columnname);
                                                string typeid = "";
                                                string length = "";
                                                string status = "";
                                                Data_dbDataHelper.GetColumnData(activeDR_2, "xtype", out typeid);
                                                Data_dbDataHelper.GetColumnData(activeDR_2, "status", out status);
                                                Data_dbDataHelper.GetColumnData(activeDR_2, "prec", out length);
                                                sql_getALLTypes = sql_getALLTypes + " where system_type_id=" + typeid;
                                                Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTypes, out TypesInfo);
                                                string typename = "";
                                                Data_dbDataHelper.GetColumnData(TypesInfo.Rows[0], "name", out typename);
                                                if (typename.Contains("nvarchar"))
                                                {
                                                    if (Int32.Parse(length) < 0)
                                                        sql_CreateNewSp.AppendLine("@" + columnname + " nvarchar(max) = null ,");
                                                    else
                                                        sql_CreateNewSp.AppendLine("@" + columnname + " nvarchar(" + length + ") = null ,");
                                                }
                                                else if (typename.Contains("char"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " char(" + length + ") = null ,");
                                                else if (typename.Contains("varcahr"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " varchar(" + length + ") = null ,");
                                                else if (typename.Contains("binary"))
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " binary(" + length + ") = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                else if (typename.Contains("varbinary"))
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " varbinary(" + length + ") = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                else if (typename.Contains("nchar"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " nchar(" + length + ") = null ,");
                                                else if (typename.Contains("decimal"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " decimal" + " = null ,");
                                                else
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " " + typename + " = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                if (typename.Contains("ntext"))
                                                    filterTypeList.Add(columnname);
                                                activeColumn.Add(columnname);
                                                if (status == "128")
                                                    activeKeyColumn.Add(columnname);
                                                if (status != "128")
                                                {
                                                    sql_insertSourceColumns.Append("[" + columnname + "],");
                                                    sql_insertValueColumns.Append("@" + columnname + ",");
                                                }
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 3, 3);
                                            if (sql_insertSourceColumns.Length > 0)
                                                sql_insertSourceColumns = sql_insertSourceColumns.Remove(sql_insertSourceColumns.Length - 1, 1);
                                            sql_insertValueColumns = sql_insertValueColumns.Remove(sql_insertValueColumns.Length - 1, 1);
                                            sql_CreateNewSp.AppendLine(")");
                                            sql_CreateNewSp.AppendLine("AS");
                                            sql_CreateNewSp.AppendLine("if @operation='select'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "]");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='insert'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("insert into " + tableName + "(" + sql_insertSourceColumns + ") values(" + sql_insertValueColumns + ")");
                                            sql_CreateNewSp.AppendLine("end");
                                            foreach (string activeCommonColumn in activeColumn)
                                            {
                                                if (!activeKeyColumn.Contains(activeCommonColumn))
                                                {
                                                    if (!filterColumn.Contains(activeCommonColumn))
                                                        sql_CreateNewSp.AppendLine("if @operation='update' and @" + activeCommonColumn + " is not null");
                                                    else
                                                        sql_CreateNewSp.AppendLine("if @operation='update'");
                                                    sql_CreateNewSp.AppendLine("begin");
                                                    sql_CreateNewSp.AppendLine("update " + tableName);
                                                    sql_CreateNewSp.AppendLine("set " + activeCommonColumn + "=@" + activeCommonColumn);
                                                    if (activeKeyColumn.Count > 0)
                                                    {
                                                        sql_CreateNewSp.AppendLine("where ");
                                                        foreach (string keyColumn in activeKeyColumn)
                                                        {
                                                            sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and");
                                                        }
                                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                                        sql_CreateNewSp.AppendLine();
                                                    }
                                                    sql_CreateNewSp.AppendLine("end");
                                                }
                                            }
                                            sql_CreateNewSp.AppendLine("else if @operation='delete'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("delete from " + tableName);
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach (string keyColumn in activeKeyColumn)
                                            {
                                                sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 5, 5);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='selectkey'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "] where ");
                                            foreach (string keyColumn in activeKeyColumn)
                                            {
                                                sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " or ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='selectcondition'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "] where ");
                                            foreach (string selectColumn in activeColumn)
                                            {
                                                if (!filterTypeList.Contains(selectColumn))
                                                    sql_CreateNewSp.Append(selectColumn + "=@" + selectColumn + " or ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='selectmixed'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "] where ");
                                            foreach (string selectColumn in activeColumn)
                                            {
                                                if (!filterTypeList.Contains(selectColumn))
                                                    sql_CreateNewSp.Append(selectColumn + "=ISNULL(@" + selectColumn + "," + selectColumn + ") and ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            Data_dbDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());

                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                else if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.MySql)
                {
                    class_data_MySqlConnectionItem mysqlActiveConnectionItem = (class_data_MySqlConnectionItem)ActiveConnection;
                    string sql_getALLTables = class_Data_SqlStringHelper.Get_SQL_GETALLTABLES_FOR_MYSQL(mysqlActiveConnectionItem.ActiveConnection.Database);
                    string name_sp = "spa_operation_";
                    DataTable TableInfo = new DataTable();
                    StringBuilder sql_CreateNewSp = new StringBuilder();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TableInfo))
                    {
                        DataTable TableSPInfos = new DataTable();
                        string sql_getALLSPInfos = class_Data_SqlStringHelper.Get_SQL_GETALLSPS_FOR_MYSQL(((class_data_MySqlConnectionItem)ActiveConnection).ActiveConnection.Database);
                        if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLSPInfos, out TableSPInfos))
                        {
                            foreach (DataRow activeSP in TableSPInfos.Rows)
                            {
                                string spname = "";
                                Data_dbDataHelper.GetColumnData(activeSP, "name", out spname);
                                string sql_dropProcedure = "drop procedure " + spname;
                                Data_dbDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_dropProcedure);
                            }
                        }
                        foreach (DataRow activeTable in TableInfo.Rows)
                        {
                            string sql_getALLColumns = "Select COLUMN_NAME,COLUMN_TYPE,COLUMN_KEY,EXTRA from INFORMATION_SCHEMA.COLUMNS Where table_schema = '{schemaname}' and table_name = '{tablename}'";
                            sql_getALLColumns = sql_getALLColumns.Replace("{schemaname}", ((class_data_MySqlConnectionItem)ActiveConnection).ActiveConnection.Database);
                            sql_CreateNewSp.Clear();
                            string tableName = "";
                            Data_dbDataHelper.GetColumnData(activeTable, "table_name", out tableName);
                            List<string> tmpSelectedColumsLst = new List<string>();
                            List<string> tmpSelectedKeyColumnsLst = new List<string>();

                            if (tableName != "")
                            {

                                sql_getALLColumns = sql_getALLColumns.Replace("{tablename}", tableName);
                                DataTable TableColumnsInfo = new DataTable();
                                if (Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLColumns, out TableColumnsInfo))
                                {
                                    sql_CreateNewSp.AppendLine("CREATE PROCEDURE " + name_sp + tableName);
                                    sql_CreateNewSp.Append("(");
                                    sql_CreateNewSp.Append("_operation varchar(40),");
                                    if (TableColumnsInfo.Rows.Count > 0)
                                    {
                                        foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                        {
                                            string column_name = "";
                                            string column_type = "";
                                            string column_extra = "";
                                            string column_key = "";
                                            Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                            Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_TYPE", out column_type);
                                            Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_KEY", out column_key);
                                            Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                            if (column_key == "PRI")
                                                tmpSelectedKeyColumnsLst.Add(column_name);
                                            sql_CreateNewSp.Append("_" + column_name + " " + column_type + ",");
                                            if (!tmpSelectedColumsLst.Contains(column_name))
                                                tmpSelectedColumsLst.Add(column_name);
                                        }
                                    }
                                    sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                    sql_CreateNewSp.AppendLine(")");
                                    sql_CreateNewSp.AppendLine("BEGIN");
                                    sql_CreateNewSp.AppendLine("DECLARE tmpsql VARCHAR(800);");
                                    sql_CreateNewSp.AppendLine("if _operation='select' then");
                                    sql_CreateNewSp.AppendLine("select * from " + tableName + ";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='insert' then");
                                    sql_CreateNewSp.Append("insert into " + ((class_data_MySqlConnectionItem)ActiveConnection).ActiveConnection.Database + "." + tableName + "(");
                                    foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                    {
                                        string column_name = "";
                                        string column_extra = "";
                                        Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                        Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                        if (column_extra != "auto_increment")
                                            sql_CreateNewSp.Append(column_name + ",");
                                    }
                                    sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                    sql_CreateNewSp.Append(")");
                                    sql_CreateNewSp.Append(" values(");
                                    foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                    {
                                        string column_name = "";
                                        string column_extra = "";
                                        Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                        Data_dbDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                        if (column_extra != "auto_increment")
                                            sql_CreateNewSp.Append("_" + column_name + ",");
                                    }
                                    sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                    sql_CreateNewSp.AppendLine(");");
                                    foreach (string activeSelectedColumn in tmpSelectedColumsLst)
                                    {
                                        if (tmpSelectedKeyColumnsLst.Contains(activeSelectedColumn))
                                            continue;
                                        sql_CreateNewSp.AppendLine("elseif _operation='update' and _" + activeSelectedColumn + " IS NOT NULL then");
                                        sql_CreateNewSp.Append("update " + tableName);
                                        sql_CreateNewSp.Append(" set " + activeSelectedColumn + " = " + "_" + activeSelectedColumn);
                                        if (tmpSelectedKeyColumnsLst.Count > 0)
                                        {
                                            sql_CreateNewSp.Append(" where ");
                                            foreach (string keyColumn in tmpSelectedKeyColumnsLst)
                                                sql_CreateNewSp.Append(keyColumn + " = _" + keyColumn + " and ");
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 5, 5);
                                            sql_CreateNewSp.AppendLine(";");
                                        }
                                    }
                                    sql_CreateNewSp.AppendLine("elseif _operation='selectmixed'then");
                                    sql_CreateNewSp.Append("select * from " + tableName + " where ");
                                    for (int i = 0; i < tmpSelectedColumsLst.Count; i++)
                                    {
                                        if (i == 0)
                                            sql_CreateNewSp.Append(tmpSelectedColumsLst[i] + " = IFNULL(_" + tmpSelectedColumsLst[i] + "," + tmpSelectedColumsLst[i] + ")");
                                        else
                                            sql_CreateNewSp.Append(" and " + tmpSelectedColumsLst[i] + " = IFNULL(_" + tmpSelectedColumsLst[i] + "," + tmpSelectedColumsLst[i] + ")");
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='delete' then");
                                    sql_CreateNewSp.Append("delete from " + tableName);
                                    if (tmpSelectedKeyColumnsLst.Count > 0)
                                    {
                                        sql_CreateNewSp.Append(" where ");
                                        foreach (string keyColumn in tmpSelectedKeyColumnsLst)
                                            sql_CreateNewSp.Append(keyColumn + " = _" + keyColumn + " and ");
                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 5, 5);
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='deletecondition' then");
                                    sql_CreateNewSp.Append("delete from " + tableName);
                                    if (tmpSelectedColumsLst.Count > 0)
                                    {
                                        sql_CreateNewSp.Append(" where ");
                                        foreach (string activeSelectedColumn in tmpSelectedColumsLst)
                                            sql_CreateNewSp.Append(activeSelectedColumn + " = _" + activeSelectedColumn + " or ");
                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='deletemixed'then");
                                    sql_CreateNewSp.Append("select * from " + tableName + " where ");
                                    for (int i = 0; i < tmpSelectedColumsLst.Count; i++)
                                    {
                                        if (i == 0)
                                            sql_CreateNewSp.Append(tmpSelectedColumsLst[i] + " = IFNULL(_" + tmpSelectedColumsLst[i] + "," + tmpSelectedColumsLst[i] + ")");
                                        else
                                            sql_CreateNewSp.Append(" and " + tmpSelectedColumsLst[i] + " = IFNULL(_" + tmpSelectedColumsLst[i] + "," + tmpSelectedColumsLst[i] + ")");
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='selectkey' then");
                                    sql_CreateNewSp.Append("select * from " + tableName);
                                    if (tmpSelectedKeyColumnsLst.Count > 0)
                                    {
                                        sql_CreateNewSp.Append(" where ");
                                        foreach (string keyColumn in tmpSelectedKeyColumnsLst)
                                            sql_CreateNewSp.Append(keyColumn + " = _" + keyColumn + " and ");
                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 5, 5);
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("elseif _operation='selectcondition' then");
                                    sql_CreateNewSp.Append("select * from " + tableName);
                                    if (tmpSelectedColumsLst.Count > 0)
                                    {
                                        sql_CreateNewSp.Append(" where ");
                                        foreach (string activeSelectedColumn in tmpSelectedColumsLst)
                                            sql_CreateNewSp.Append(activeSelectedColumn + " = _" + activeSelectedColumn + " or ");
                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                    }
                                    sql_CreateNewSp.AppendLine(";");
                                    sql_CreateNewSp.AppendLine("END IF;");
                                    sql_CreateNewSp.AppendLine("END");
                                    Data_dbDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());
                                }
                            }

                        }
                    }
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

        public List<string> ActionGetAllUserTables(class_data_PlatformDBConnection ActiveConnection)
        {
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = class_Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;
                if (ActiveConnection != null)
                {
                    DataTable TablesInfo = new DataTable();
                    Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        Data_dbDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                        result.Add(tableName);
                    }
                    return result;

                }
                else
                    return result;
            }
            catch (Basic_Exceptions err)
            {
                return result;
            }
        }

        public List<string> ActionGetAllUserStoreProcs(class_data_PlatformDBConnection ActiveConnection)
        {
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = "select * from sys.all_objects where type='P' and is_ms_shipped=0";
                if (ActiveConnection != null)
                {
                    DataTable TablesInfo = new DataTable();
                    Data_dbDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        Data_dbDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                        result.Add(tableName);
                    }
                    return result;

                }
                else
                    return result;
            }
            catch (Basic_Exceptions err)
            {
                return result;
            }
        }

        public string ActionBuildCreateSqlString(List<string> activeTableStructs, string tableName, string activeDBName)
        {
            if (activeTableStructs.Count == 0)
                return "";
            else
            {
                StringBuilder sql_Result = new StringBuilder();
                sql_Result.AppendLine("USE [" + activeDBName + "]");
                sql_Result.AppendLine("CREATE TABLE " + tableName);
                sql_Result.AppendLine("(");
                foreach (string activeTableStruct in activeTableStructs)
                {
                    sql_Result.AppendLine(activeTableStruct);
                }
                sql_Result.AppendLine(")");
                return sql_Result.ToString();
            }
        }

        public bool ActionExecuteCreateSql(List<string> activeTableStructes, string activeDBName, string tableName, class_data_PlatformDBConnection activeConnection)
        {
            try
            {
                if (activeTableStructes.Count == 0 || activeDBName == "" || activeConnection == null)
                    return false;
                else
                {
                    StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'" + tableName + "'), N'IsTable') = 1");
                    sql_CreateNewSp.AppendLine();
                    sql_CreateNewSp.AppendLine("DROP TABLE " + tableName);
                    Data_dbDataHelper.ActionExecuteForNonQuery(activeConnection, sql_CreateNewSp.ToString());
                    string sql = ActionBuildCreateSqlString(activeTableStructes, tableName, activeDBName);
                    if (sql != "")
                    {
                        if (Data_dbDataHelper.ActionExecuteForNonQuery(activeConnection, sql))
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            catch (Basic_Exceptions err)
            {
                return false;
            }
        }


        public Dictionary<string, class_Data_SqlSPEntry> ActionAutoLoadingAllSPS(class_data_PlatformDBConnection activeConnection, string SPType)
        {
            if (activeConnection != null)
            {
                Dictionary<string, class_Data_SqlSPEntry> result = new Dictionary<string, class_Data_SqlSPEntry>();
                if (activeConnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                {
                    string sql_getallsps = "select * from sys.all_objects where (type = 'P') AND (is_ms_shipped = 0)";
                    DataTable activeSPSDT = new DataTable();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_getallsps, out activeSPSDT))
                    {
                        foreach (DataRow activeRow in activeSPSDT.Rows)
                        {
                            class_Data_SqlSPEntry newSPEntry = new class_Data_SqlSPEntry(activeConnection.activeDatabaseType);
                            string spName = "";
                            Data_dbDataHelper.GetColumnData(activeRow, "name", out spName);
                            if (SPType != "")
                            {
                                if (SPType == class_Data_SqlSPEntryType.SelectAction)
                                {
                                    if (!spName.StartsWith(class_Data_SqlSPEntryNameFiler.StartNamed_SelectAction))
                                        continue;
                                }
                                else if (SPType == class_Data_SqlSPEntryType.UpdateAction)
                                {
                                    if (!spName.StartsWith(class_Data_SqlSPEntryNameFiler.StartNamed_Update))
                                        continue;
                                }
                            }
                            newSPEntry.SPName = spName;
                            newSPEntry.KeyName = spName;
                            string spObjectID = "";
                            Data_dbDataHelper.GetColumnData(activeRow, "object_id", out spObjectID);
                            string sql_paramters = "select * from sys.all_parameters where object_id = " + spObjectID;
                            DataTable activeSPParametersDT = new DataTable();
                            string sql_paramstype = "select * from sys.types";
                            DataTable paramstypeDT = new DataTable();
                            if (!Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramstype, out paramstypeDT))
                            {
                                return null;
                            }
                            if (Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramters, out activeSPParametersDT))
                            {
                                foreach (DataRow activeParamterRow in activeSPParametersDT.Rows)
                                {
                                    string activeSystemType_ID = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "system_type_id", out activeSystemType_ID);
                                    string activeUserType_ID = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "user_type_id", out activeUserType_ID);
                                    string activeParamsMaxLength = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "max_length", out activeParamsMaxLength);
                                    string activeParamsName = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "name", out activeParamsName);
                                    string activeIsOutPut = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "is_output", out activeIsOutPut);
                                    string max_length = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "max_length", out max_length);
                                    string activeDBType = "";
                                    DataRow[] dbtyps = paramstypeDT.Select("system_type_id=" + activeSystemType_ID + " and user_type_id=" + activeUserType_ID);
                                    if (dbtyps.Length > 0)
                                    {
                                        Data_dbDataHelper.GetColumnData(dbtyps[0], "name", out activeDBType);
                                        ((class_data_SqlServerSPEntry)newSPEntry).SetNewParameter(activeParamsName, Util_Data.ConventStrTODbtye(activeDBType), ParameterDirection.Input, int.Parse(max_length), null);
                                    }
                                    else
                                        continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                            result.Add(newSPEntry.KeyName, newSPEntry);
                        }
                    }
                }
                else if (activeConnection.activeDatabaseType == enum_DatabaseType.MySql)
                {
                    string sql_getALLSPInfo = "select name,param_list from mysql.proc where db = '{schemaname}' and type = 'PROCEDURE'";
                    sql_getALLSPInfo = sql_getALLSPInfo.Replace("{schemaname}", ((class_data_MySqlConnectionItem)activeConnection).ActiveConnection.Database);
                    DataTable dtALLSPInfo = new DataTable();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT((class_data_MySqlConnectionItem)activeConnection, sql_getALLSPInfo, out dtALLSPInfo))
                    {
                        foreach (DataRow activeDR in dtALLSPInfo.Rows)
                        {
                            class_data_MySqlSPEntry newSPEntry = new class_data_MySqlSPEntry(activeConnection.activeDatabaseType);
                            string spName = "";
                            Data_dbDataHelper.GetColumnData(activeDR, "name", out spName);
                            newSPEntry.SPName = spName;
                            newSPEntry.KeyName = spName;
                            string tmpParamsFromDB = "";
                            Data_dbDataHelper.GetArrByteColumnDataToString(activeDR, "param_list", out tmpParamsFromDB);
                            string[] activeParams = tmpParamsFromDB.Split(',');
                            foreach (string activeParam in activeParams)
                            {
                                string[] activeParamInfo = activeParam.Split(' ');
                                if (activeParamInfo.Length >= 2)
                                {
                                    string parameterName = activeParamInfo[0];
                                    string parameterType = activeParamInfo[1].Split('(')[0];
                                    string parameterLength = "0";
                                    try
                                    {
                                        parameterLength = activeParamInfo[1].Split('(')[1].Replace(")", "");
                                    }
                                    catch
                                    {

                                    }
                                    int activeParameterLength = 0;
                                    int.TryParse(parameterLength, out activeParameterLength);
                                    ((class_data_MySqlSPEntry)newSPEntry).SetNewParameter(activeParamInfo[0], Util_Data.ConventStrTOMySqlDbtye(parameterType), ParameterDirection.Input, activeParameterLength, null);
                                }
                            }
                            result.Add(newSPEntry.KeyName, newSPEntry);
                        }
                    }
                }
                return result;
            }
            else
                return null;
        }


        public class_Data_SqlSPEntry ActionLoadingActiveSPS(class_data_PlatformDBConnection activeConnection, string activeSPName)
        {
            if (activeConnection != null)
            {
                if (activeConnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                {
                    string sql_getallsps = "select * from sys.all_objects where (type = 'P') AND (is_ms_shipped = 0) AND (name='" + activeSPName + "')";
                    DataTable activeSPSDT = new DataTable();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_getallsps, out activeSPSDT))
                    {
                        if (activeSPSDT != null && activeSPSDT.Rows.Count > 0)
                        {
                            class_Data_SqlSPEntry newSPEntry = new class_Data_SqlSPEntry(activeConnection.activeDatabaseType);
                            string spName = "";
                            Data_dbDataHelper.GetColumnData(activeSPSDT.Rows[0], "name", out spName);
                            newSPEntry.SPName = spName;
                            newSPEntry.KeyName = spName;
                            string spObjectID = "";
                            Data_dbDataHelper.GetColumnData(activeSPSDT.Rows[0], "object_id", out spObjectID);
                            string sql_paramters = "select * from sys.all_parameters where object_id = " + spObjectID;
                            DataTable activeSPParametersDT = new DataTable();
                            string sql_paramstype = "select * from sys.types";
                            DataTable paramstypeDT = new DataTable();
                            if (!Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramstype, out paramstypeDT))
                            {
                                return null;
                            }
                            if (Data_dbDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramters, out activeSPParametersDT))
                            {
                                foreach (DataRow activeParamterRow in activeSPParametersDT.Rows)
                                {
                                    string activeSystemType_ID = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "system_type_id", out activeSystemType_ID);
                                    string activeUserType_ID = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "user_type_id", out activeUserType_ID);
                                    string activeParamsMaxLength = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "max_length", out activeParamsMaxLength);
                                    string activeParamsName = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "name", out activeParamsName);
                                    string activeIsOutPut = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "is_output", out activeIsOutPut);
                                    string max_length = "";
                                    Data_dbDataHelper.GetColumnData(activeParamterRow, "max_length", out max_length);
                                    string activeDBType = "";
                                    DataRow[] dbtyps = paramstypeDT.Select("system_type_id=" + activeSystemType_ID + " and user_type_id=" + activeUserType_ID);
                                    if (dbtyps.Length > 0)
                                    {
                                        Data_dbDataHelper.GetColumnData(dbtyps[0], "name", out activeDBType);
                                        ((class_data_SqlServerSPEntry)newSPEntry).SetNewParameter(activeParamsName, Util_Data.ConventStrTODbtye(activeDBType), ParameterDirection.Input, int.Parse(max_length), null);
                                    }
                                    else
                                        continue;
                                }
                            }
                            return newSPEntry;
                        }
                    }
                }
                else if (activeConnection.activeDatabaseType == enum_DatabaseType.MySql)
                {
                    string sql_getALLSPInfo = "select name,param_list from mysql.proc where db = '{schemaname}' and type = 'PROCEDURE' and name='" + activeSPName + "'";
                    sql_getALLSPInfo = sql_getALLSPInfo.Replace("{schemaname}", ((class_data_MySqlConnectionItem)activeConnection).ActiveConnection.Database);
                    DataTable dtALLSPInfo = new DataTable();
                    if (Data_dbDataHelper.ActionExecuteSQLForDT((class_data_MySqlConnectionItem)activeConnection, sql_getALLSPInfo, out dtALLSPInfo))
                    {
                        if (dtALLSPInfo != null && dtALLSPInfo.Rows.Count > 0)
                        {
                            class_data_MySqlSPEntry newSPEntry = new class_data_MySqlSPEntry(activeConnection.activeDatabaseType);
                            string spName = "";
                            Data_dbDataHelper.GetColumnData(dtALLSPInfo.Rows[0], "name", out spName);
                            newSPEntry.SPName = spName;
                            newSPEntry.KeyName = spName;
                            string tmpParamsFromDB = "";
                            Data_dbDataHelper.GetArrByteColumnDataToString(dtALLSPInfo.Rows[0], "param_list", out tmpParamsFromDB);
                            string[] activeParams = tmpParamsFromDB.Split(',');
                            foreach (string activeParam in activeParams)
                            {
                                string[] activeParamInfo = activeParam.Split(' ');
                                if (activeParamInfo.Length >= 2)
                                {
                                    string parameterName = activeParamInfo[0];
                                    string parameterType = activeParamInfo[1].Split('(')[0];
                                    string parameterLength = "0";
                                    try
                                    {
                                        parameterLength = activeParamInfo[1].Split('(')[1].Replace(")", "");
                                    }
                                    catch
                                    {

                                    }
                                    int activeParameterLength = 0;
                                    int.TryParse(parameterLength, out activeParameterLength);
                                    ((class_data_MySqlSPEntry)newSPEntry).SetNewParameter(activeParamInfo[0], Util_Data.ConventStrTOMySqlDbtye(parameterType), ParameterDirection.Input, activeParameterLength, null);
                                }
                            }
                            return newSPEntry;
                        }
                    }
                }
                return null;
            }
            else
                return null;
        }


        public DataTable ExecuteSelectSPKeyForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectkey");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectkey");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPConditionForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectcondition");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectcondition");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPConditionForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectcondition");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectcondition");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPMixedConditionsForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectmixed");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectmixed");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPMixedConditionsForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectmixed");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectmixed");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPKeyForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "selectkey");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "selectkey");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "select");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "select");
                Data_dbDataHelper activeSqlSPHelper = new Data_dbDataHelper();
                Data_dbDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "select");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "select");
                DataTable resultDT = null;
                Data_dbDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out resultDT);
                return resultDT;
            }
            else
                return null;
        }

        public bool ExecuteInsertSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "insert");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "insert");
                Data_dbDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteUpdateSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "update");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "update");
                Data_dbDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteDeleteSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "delete");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "delete");
                Data_dbDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteDeleteConditionSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "deletecondition");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "deletecondition");
                Data_dbDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteDeleteMixedSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.SqlServer)
                    ((class_data_SqlServerSPEntry)activeEntry).ModifyParameterValue("@operation", "deletemixed");
                else if (connectionHelper.Get_ActiveConnection(connectionKeyName).activeDatabaseType == enum_DatabaseType.MySql)
                    ((class_data_MySqlSPEntry)activeEntry).ModifyParameterValue("_operation", "deletemixed");
                Data_dbDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

    }

}
