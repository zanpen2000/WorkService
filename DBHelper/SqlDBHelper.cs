using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBHelper
{
    public class SqlDBHelper : IDBHelper
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
        public SqlDBHelper(string conStr)
        {
            connctionString = conStr;//= System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
        private SqlConnection getConnection()
        {
            return new SqlConnection(connctionString);
        }
        /// <summary>
        /// 执行不带带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public System.Data.DataTable Query(string sql)
        {
            using (SqlConnection conn = getConnection())
            {
                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter sqlAdapter1 = new SqlDataAdapter(sql, conn);
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
        /// <summary>
        /// 执行带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public System.Data.DataTable Query(string sql, params IDataParameter[] cmdParms)
        {
            using (SqlConnection conn = getConnection())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    PrepareCommand(cmd, conn, trans, sql, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
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

        public DataSet QueryDataSet(string sql)
        {
            using (SqlConnection conn = getConnection())
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlDataAdapter sqlAdapter1 = new SqlDataAdapter(sql, conn);
                    sqlAdapter1.Fill(ds);
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

        public DataSet QueryDataSet(string sql, params IDataParameter[] cmdParms)
        {
            using (SqlConnection conn = getConnection())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    PrepareCommand(cmd, conn, trans, sql, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            da.Fill(ds);
                            trans.Commit();
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
        }
        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public object QueryScalar(string sql, params IDataParameter[] cmdParms)
        {
            SqlConnection conn = getConnection();
            SqlCommand cmd = new SqlCommand();
            try
            {
                conn.Open();
                //SqlTransaction trans = conn.BeginTransaction();
                PrepareCommand(cmd, conn, null, sql, cmdParms);
                object obj = cmd.ExecuteScalar();
                conn.Close();
                return obj;
            }
            catch { return null; }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 执行不带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public System.Data.IDataReader QueryReader(string sql)
        {
            SqlConnection connection = getConnection();
            SqlCommand cmd = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public System.Data.IDataReader QueryReader(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            SqlConnection connection = getConnection();
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, sql, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public int Execute(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            using (SqlConnection conn = getConnection())
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();
                    try
                    {
                        PrepareCommand(cmd, conn, tran, sql, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        tran.Commit();
                        return rows;
                    }
                    catch
                    {
                        tran.Rollback();
                        return -1;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public System.Data.DataSet ExecuteStoredProcedures(string name, params System.Data.IDataParameter[] cmdParms)
        {
            using (SqlConnection conn = getConnection())
            {
                using (SqlCommand command = new SqlCommand())
                {
                    DataSet ds = new DataSet();
                    BuildQueryCommand(command, conn, name, cmdParms);
                    SqlDataAdapter sda = new SqlDataAdapter(command);
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

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public IDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection conn = getConnection();
            SqlDataReader returnReader = null;
            conn.Open();
            SqlCommand command = new SqlCommand();
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
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name">存储过程名称</param>
        /// <param name="count">返回值的数组位置</param>
        /// <param name="cmdParms">参数数组</param>
        /// <returns></returns>
        public string DHExecuteStoredProcedures(string name, int count, params System.Data.IDataParameter[] cmdParms)
        {
            string info;
            using (SqlConnection conn = getConnection())
            {
                using (SqlCommand command = new SqlCommand(name, conn))
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
        /// <summary>
        /// 执行批量SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParmsList"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, List<System.Data.IDataParameter[]> cmdParmsList)
        {
            int i = 0;
            using (SqlConnection conn = getConnection())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();
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
        /// <summary>
        /// 构建 SqlCommand 对象
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private void BuildQueryCommand(SqlCommand cmd, SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            cmd.Connection = connection;
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
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
        /// <summary>
        /// 创建command、SqlParameter对象
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, IDataParameter[] cmdParms)
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
                foreach (SqlParameter parameter in cmdParms)
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
        /// 执行分页存储过程
        /// </summary>
        /// <param name="tableName">要查询的表</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="orderFields">用于排序的字段</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageCount">页面总数</param>
        /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
        /// <param name="where">不要加where 关键字</param>
        /// <returns></returns>
        public IDataReader SP_PagingByReader(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            string sql = "SP_Paging";
            SqlParameter[] paras = { 
                                   new SqlParameter("@tblName",SqlDbType.VarChar,255),
                                   new SqlParameter("@strGetFields",SqlDbType.VarChar,1000),
                                   new SqlParameter("@fldName",SqlDbType.VarChar,255),
                                   new SqlParameter("@pageIndex",SqlDbType.Int,4),
                                   new SqlParameter("@pageRow",SqlDbType.Int,4),

                                   new SqlParameter("@recordCount",SqlDbType.Int,4),
                                   new SqlParameter("@pageCount",SqlDbType.Int,4),

                                   new SqlParameter("@orderType",SqlDbType.Int,4),
                                   new SqlParameter("@strWhere",SqlDbType.VarChar,1000)
                                   };
            paras[0].Value = tableName;
            paras[1].Value = fields;
            paras[2].Value = orderFields;
            paras[3].Value = pageIndex;
            paras[4].Value = pageSize;
            paras[5].Direction = ParameterDirection.Output;
            paras[6].Direction = ParameterDirection.Output;
            paras[7].Value = orderType;
            paras[8].Value = where;
            paras[5].Value = 0;
            paras[6].Value = 0;

            IDataReader reader = RunProcedure(sql, paras);
            if (paras[5].Value != null)
            {
                recordCount = int.Parse(paras[5].Value.ToString());
            }
            if (paras[6].Value != null)
            {
                pageCount = int.Parse(paras[6].Value.ToString());
            }
            return reader;
        }
        /// <summary>
        /// 执行分页存储过程
        /// </summary>
        /// <param name="tableName">要查询的表</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="orderFields">用于排序的字段</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageCount">页面总数</param>
        /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
        /// <param name="where">不要加where 关键字</param>
        /// <returns></returns>
        public DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            string sql = "SP_Paging";
            SqlParameter[] paras = { 
                                   new SqlParameter("@tblName",SqlDbType.VarChar,255),
                                   new SqlParameter("@strGetFields",SqlDbType.VarChar,1000),
                                   new SqlParameter("@fldName",SqlDbType.VarChar,255),
                                   new SqlParameter("@pageIndex",SqlDbType.Int,4),
                                   new SqlParameter("@pageRow",SqlDbType.Int,4),
                                   new SqlParameter("@recordCount",SqlDbType.Int,4),
                                   new SqlParameter("@pageCount",SqlDbType.Int,4),
                                   new SqlParameter("@orderType",SqlDbType.Int,4),
                                   new SqlParameter("@strWhere",SqlDbType.VarChar,1000)
                                   };
            paras[0].Value = tableName;
            paras[1].Value = fields;
            paras[2].Value = orderFields;
            paras[3].Value = pageIndex;
            paras[4].Value = pageSize;
            paras[5].Direction = ParameterDirection.Output;
            paras[6].Direction = ParameterDirection.Output;
            paras[7].Value = orderType;
            paras[8].Value = where;

            DataSet ds = ExecuteStoredProcedures(sql, paras);
            if (paras[5].Value != null)
            {
                recordCount = int.Parse(paras[5].Value.ToString());
            }
            if (paras[6].Value != null)
            {
                pageCount = int.Parse(paras[6].Value.ToString());
            }
            return ds.Tables[0];
        }
        /// <summary>
        /// 执行分页存储过程（带合计行）
        /// </summary>
        /// <param name="tableName">要查询的表</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="orderFields">用于排序的字段</param>
        /// <param name="id">唯一索引字段</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageCount">页面总数</param>
        /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
        /// <param name="where">不要加where 关键字</param>
        /// <param name="totalFields">参与计算合计的字段，用逗号隔开</param>
        /// <returns></returns>
        public DataSet SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where,string totalFields)
        {
            string sql = "WD_Page";
            SqlParameter[] paras = { 
                                   new SqlParameter("@tblName",SqlDbType.VarChar,255),
                                   new SqlParameter("@strGetFields",SqlDbType.VarChar,1000),
                                   new SqlParameter("@fldName",SqlDbType.VarChar,255),
                                   new SqlParameter("@id",SqlDbType.VarChar,50),
                                   new SqlParameter("@pageIndex",SqlDbType.Int,4),
                                   new SqlParameter("@pageRow",SqlDbType.Int,4),
                                   new SqlParameter("@recordCount",SqlDbType.Int,4),
                                   new SqlParameter("@pageCount",SqlDbType.Int,4),
                                   new SqlParameter("@orderType",SqlDbType.Int,4),
                                   new SqlParameter("@strWhere",SqlDbType.VarChar),
                                   new SqlParameter("@totalFields",SqlDbType.VarChar)
                                   };
            paras[0].Value = tableName;
            paras[1].Value = fields;
            paras[2].Value = orderFields;
            paras[3].Value = id;
            paras[4].Value = pageIndex;
            paras[5].Value = pageSize;
            paras[6].Direction = ParameterDirection.Output;
            paras[7].Direction = ParameterDirection.Output;
            paras[8].Value = orderType;
            paras[9].Value = where;
            paras[10].Value = totalFields;

            DataSet ds = ExecuteStoredProcedures(sql, paras);
            if (paras[6].Value != null)
            {
                recordCount = int.Parse(paras[6].Value.ToString());
            }
            if (paras[7].Value != null)
            {
                pageCount = int.Parse(paras[7].Value.ToString());
            }
            return ds;
        }

        /// <summary>
        /// 执行分页存储过程(不带合计行)
        /// </summary>
        /// <param name="tableName">要查询的表</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="orderFields">用于排序的字段</param>
        /// <param name="id">唯一索引字段</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageCount">页面总数</param>
        /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
        /// <param name="where">不要加where 关键字</param>
        /// <param name="totalFields">参与计算合计的字段，用逗号隔开</param>
        /// <returns></returns>
        public DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            string sql = "WD_Page";
            SqlParameter[] paras = { 
                                   new SqlParameter("@tblName",SqlDbType.VarChar,255),
                                   new SqlParameter("@strGetFields",SqlDbType.VarChar,1000),
                                   new SqlParameter("@fldName",SqlDbType.VarChar,255),
                                   new SqlParameter("@id",SqlDbType.VarChar,50),
                                   new SqlParameter("@pageIndex",SqlDbType.Int,4),
                                   new SqlParameter("@pageRow",SqlDbType.Int,4),
                                   new SqlParameter("@recordCount",SqlDbType.Int,4),
                                   new SqlParameter("@pageCount",SqlDbType.Int,4),
                                   new SqlParameter("@orderType",SqlDbType.Int,4),
                                   new SqlParameter("@strWhere",SqlDbType.VarChar),
                                   new SqlParameter("@totalFields",SqlDbType.VarChar)
                                   };
            paras[0].Value = tableName;
            paras[1].Value = fields;
            paras[2].Value = orderFields;
            paras[3].Value = id;
            paras[4].Value = pageIndex;
            paras[5].Value = pageSize;
            paras[6].Direction = ParameterDirection.Output;
            paras[7].Direction = ParameterDirection.Output;
            paras[8].Value = orderType;
            paras[9].Value = where;
            paras[10].Value = "";

            DataSet ds = ExecuteStoredProcedures(sql, paras);
            if (paras[6].Value != null)
            {
                recordCount = int.Parse(paras[6].Value.ToString());
            }
            if (paras[7].Value != null)
            {
                pageCount = int.Parse(paras[7].Value.ToString());
            }
            return ds.Tables[0];
        }
        
        
        /// <summary>
        /// 读取excel中的数据
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public DataSet LoadDataFromExcel(string File, string sheetName)
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
                    da.Fill(ds, sheetName);
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
            using (SqlConnection conn = getConnection())
            {
                conn.Open();
                SqlTransaction sqlbulkTransaction = conn.BeginTransaction();
                //请在插入数据的同时检查约束，如果发生错误调用sqlbulkTransaction事务
                SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, sqlbulkTransaction);
                copy.DestinationTableName = tableName;
                foreach (DataColumn dc in dt.Columns)
                {
                    copy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                try
                {
                    copy.WriteToServer(dt);
                    sqlbulkTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    sqlbulkTransaction.Rollback();
                    throw ex;
                    //Console.WriteLine(ex.ToString());
                    //return false;
                }
                finally
                {
                    copy.Close();
                    conn.Close();
                }
            }
            
        }
    }
}
