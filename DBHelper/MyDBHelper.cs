using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DBHelper
{
    public static class MyDBHelper
    {
        static MyDBHelper()
        {
            InitConnectionString(); 
        }

        public static string ConnctionString = "";
        private static IDBHelper dbhelper = null;
        /// <summary>
        /// 当前系统数据库的类型
        /// </summary>
        public static MyDbType DbType = MyDbType.SqlServer;
        public static void  InitConnectionString(string constr)
        {
            ConnctionString = constr;
            if (string.IsNullOrEmpty(ConnctionString))
            {
                ConnctionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            if (ConnctionString.IndexOf("Initial Catalog", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DbType = MyDbType.SqlServer;
                dbhelper = new SqlDBHelper(ConnctionString);
            }
            else if (ConnctionString.IndexOf("CONNECT_DATA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DbType = MyDbType.Oracle;
                dbhelper = null;
            }
            else
            {
                dbhelper = null;
            }
        }

        public static void InitConnectionString()
        {
            ConnctionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            if (string.IsNullOrEmpty(ConnctionString))
            {
                ConnctionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            if (ConnctionString.IndexOf("Initial Catalog", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DbType = MyDbType.SqlServer;
                dbhelper = new SqlDBHelper(ConnctionString);
            }
            else if (ConnctionString.IndexOf("CONNECT_DATA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DbType = MyDbType.Oracle;
                dbhelper = null;
            }
            else
            {
                dbhelper = null;
            }
        }

        /// <summary>
        /// 执行不带带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static System.Data.DataTable Query(string sql)
        {
            return dbhelper.Query(sql);
        }
        /// <summary>
        /// 执行带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static System.Data.DataTable Query(string sql, params IDataParameter[] cmdParms)
        {
            return dbhelper.Query(sql, cmdParms);
        }
        /// <summary>
        /// 执行不带带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static System.Data.DataSet QueryDataSet(string sql)
        {
            return dbhelper.QueryDataSet(sql);
        }
        /// <summary>
        /// 执行带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static System.Data.DataSet QueryDataSet(string sql, params IDataParameter[] cmdParms)
        {
            return dbhelper.QueryDataSet(sql, cmdParms);
        }

        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object QueryScalar(string sql, params IDataParameter[] cmdParms)
        {
            return dbhelper.QueryScalar(sql, cmdParms);
        }
        /// <summary>
        /// 执行不带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static System.Data.IDataReader QueryReader(string sql)
        {
            return dbhelper.QueryReader(sql);
        }
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static System.Data.IDataReader QueryReader(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            return dbhelper.QueryReader(sql, cmdParms);
        }
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int Execute(string sql, params System.Data.IDataParameter[] cmdParms)
        {
            return dbhelper.Execute(sql, cmdParms);
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static System.Data.DataSet ExecuteStoredProcedures(string name, params System.Data.IDataParameter[] cmdParms)
        {
            return dbhelper.ExecuteStoredProcedures(name, cmdParms);
        }
        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static IDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            return dbhelper.RunProcedure(storedProcName, parameters);

        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name">存储过程名称</param>
        /// <param name="count">返回值的数组位置</param>
        /// <param name="cmdParms">参数数组</param>
        /// <returns></returns>
        public static string DHExecuteStoredProcedures(string name, int count, params System.Data.IDataParameter[] cmdParms)
        {
            return dbhelper.DHExecuteStoredProcedures(name, count, cmdParms);
        }
        /// <summary>
        /// 执行批量SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParmsList"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql, List<System.Data.IDataParameter[]> cmdParmsList)
        {
            return dbhelper.ExecuteSql(sql, cmdParmsList);
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
        private static IDataReader SP_PagingByReader(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            return dbhelper.SP_PagingByReader(tableName,  fields,  orderFields,  pageIndex,  pageSize,  ref recordCount,  ref pageCount,  orderType,  where);
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
        private static DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            return dbhelper.SP_PageingByDataset(tableName, fields, orderFields, pageIndex, pageSize, ref recordCount, ref pageCount, orderType, where);
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
        /// <returns></returns>
        public static DataSet SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where, string totalFields)
        {
            return dbhelper.SP_PageingByDataset(tableName, fields, orderFields, id, pageIndex, pageSize, ref recordCount, ref pageCount, orderType, where, totalFields);
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
        /// <returns></returns>
        public static DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where)
        {
            return dbhelper.SP_PageingByDataset(tableName, fields, orderFields, id, pageIndex, pageSize, ref recordCount, ref pageCount, orderType, where);
        }
        /// <summary>
        /// 读取excel中的数据
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static DataSet LoadDataFromExcel(string File, string sheetName)
        {
            return dbhelper.LoadDataFromExcel(File, sheetName);
        }
        /// <summary>
        /// 使用SqlBulkCopy大批量插入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool AddDataBySqlBulkCopy(string tableName, DataTable dt)
        {
            return dbhelper.AddDataBySqlBulkCopy(tableName,dt);
        }
    }
    public enum MyDbType
    {
        SqlServer,
        Oracle,
        None
    }
}
