using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.OracleClient;
namespace DBHelper
{
    internal class Oracl2DBHelper : IDBHelper
    {
        private string connctionString = "";
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnctionString
        {
            get { return connctionString; }
            set { connctionString = value; }
        }

        public Oracl2DBHelper(string conStr)
        {
            connctionString = conStr;
        }
        private OracleConnection getConnection()
        {
            return new OracleConnection(connctionString);
        }
        public System.Data.DataTable Query(string sql)
        {
            using (OracleConnection conn = getConnection())
            {
                try
                {
                    DataTable dt = new DataTable();
                    OracleDataAdapter sqlAdapter1 = new OracleDataAdapter(sql, conn);
                    sqlAdapter1.Fill(dt);
                    return dt;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public System.Data.DataTable Query(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            using (OracleConnection conn = getConnection())
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    conn.Open();
                    OracleTransaction trans = conn.BeginTransaction();
                    PrepareCommand(cmd, conn, trans, sql, cmdParms);
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            da.Fill(dt);
                            trans.Commit();
                            return dt;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            throw e;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }

        public object QueryScalar(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            OracleConnection conn = getConnection();
            OracleCommand cmd = new OracleCommand();
            try
            {
                conn.Open();
                OracleTransaction trans = conn.BeginTransaction();
                PrepareCommand(cmd, conn, trans, sql, cmdParms);
                object obj = cmd.ExecuteScalar();
                trans.Commit();
                conn.Close();
                return obj;
            }
            catch { return null; }
            finally
            {
                conn.Close();
            }
        }

        public System.Data.IDataReader QueryReader(string sql)
        {
            OracleConnection connection = getConnection();
            OracleCommand cmd = new OracleCommand(sql, connection);
            try
            {
                connection.Open();
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }

        public System.Data.IDataReader QueryReader(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            OracleConnection connection = getConnection();
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, sql, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }

        public int Execute(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            using (OracleConnection conn = getConnection())
            {

                using (OracleCommand cmd = new OracleCommand())
                {
                    conn.Open();
                    OracleTransaction tran = conn.BeginTransaction();
                    try
                    {
                        PrepareCommand(cmd, conn, tran, sql, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        tran.Commit();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public System.Data.DataSet ExecuteStoredProcedures(string name, params System.Data.IDataParameter[] cmdParms)
        {
            using (OracleConnection conn = getConnection())
            {
                using (OracleCommand command = new OracleCommand())
                {
                    DataSet ds = new DataSet();
                    BuildQueryCommand(command, conn, name, cmdParms);
                    OracleDataAdapter sda = new OracleDataAdapter(command);
                    try
                    {
                        conn.Open();
                        sda.Fill(ds);
                        return ds;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }
        }

        public System.Data.IDataReader RunProcedure(string storedProcName, System.Data.IDataParameter[] parameters)
        {
            OracleConnection conn = getConnection();
            OracleDataReader returnReader = null;
            conn.Open();
            OracleCommand command = new OracleCommand();
            BuildQueryCommand(command, conn, storedProcName, parameters);
            try
            {
                returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return returnReader;
        }

        public string DHExecuteStoredProcedures(string name, int count, params System.Data.IDataParameter[] cmdParms)
        {
            string info;
            using (OracleConnection conn = getConnection())
            {
                using (OracleCommand command = new OracleCommand(name, conn))
                {
                    try
                    {
                        conn.Open();
                        BuildQueryCommand(command, conn, name, cmdParms);
                        command.ExecuteNonQuery();
                        info = (string)command.Parameters[count].Value;
                        return info;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public int ExecuteSql(string sql, List<System.Data.IDataParameter[]> cmdParmsList)
        {
            int i = 0;
            using (OracleConnection conn = getConnection())
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    conn.Open();
                    OracleTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    foreach (IDataParameter[] cmdParms in cmdParmsList)
                    {
                        foreach (IDataParameter id in cmdParms)
                        {
                            cmd.Parameters.Add(id);
                        }
                        try
                        {
                            i += cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            break;
                        }
                        cmd.Parameters.Clear();
                    }
                    if (i == cmdParmsList.Count)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                }
            }
            return i;
        }

        public System.Data.IDataReader SP_PagingByReader(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            return null;
        }

        public System.Data.DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            return null;
        }

        public System.Data.DataSet LoadDataFromExcel(string File, string sheetName)
        {
            try
            {
                string connectionString;
                DataSet ds = new DataSet();
                if (File != "")
                {
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + File + ";Extended Properties='Excel 8.0;HDR=False;IMEX=1'";
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    String strQuery = "SELECT * FROM  [" + sheetName + "$]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strQuery, conn);
                    da.Fill(ds, "Sheet1");
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 使用SqlBulkCopy大批量插入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool AddDataBySqlBulkCopy(string tableName, DataTable dt)
        {
            return false;
        }
        /// <summary>
        /// 创建command、SqlParameter对象
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, IDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        /// <summary>
        /// 构建 SqlCommand 对象
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private void BuildQueryCommand(OracleCommand cmd, OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            cmd.Connection = connection;
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }


        public DataSet SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where, string totalFields)
        {
            throw new NotImplementedException();
        }
        public DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            throw new NotImplementedException();
        }

        public DataSet QueryDataSet(string sql)
        {
            throw new NotImplementedException();
        }

        public DataSet QueryDataSet(string sql, params IDataParameter[] cmdParms)
        {
            throw new NotImplementedException();
        }
    }
}
