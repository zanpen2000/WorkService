using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DBHelper
{
    internal interface IDBHelper
    {
        /// <summary>
        /// 执行不带带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
         System.Data.DataTable Query(string sql);
        /// <summary>
        /// 执行带参数的sql查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         System.Data.DataTable Query(string sql, params IDataParameter[] cmdParms);
        /// <summary>
        /// 执行不带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
         System.Data.DataSet QueryDataSet(string sql);
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         System.Data.DataSet QueryDataSet(string sql, params IDataParameter[] cmdParms);
        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         object QueryScalar(string sql, params IDataParameter[] cmdParms);
        /// <summary>
        /// 执行不带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
         System.Data.IDataReader QueryReader(string sql);
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         System.Data.IDataReader QueryReader(string sql, params System.Data.IDataParameter[] cmdParms);
        /// <summary>
        /// 执行带参数的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         int Execute(string sql, params System.Data.IDataParameter[] cmdParms);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
         System.Data.DataSet ExecuteStoredProcedures(string name, params System.Data.IDataParameter[] cmdParms);
        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
         IDataReader RunProcedure(string storedProcName, IDataParameter[] parameters);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name">存储过程名称</param>
        /// <param name="count">返回值的数组位置</param>
        /// <param name="cmdParms">参数数组</param>
        /// <returns></returns>
         string DHExecuteStoredProcedures(string name, int count, params System.Data.IDataParameter[] cmdParms);
        /// <summary>
        /// 执行批量SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParmsList"></param>
        /// <returns></returns>
         int ExecuteSql(string sql, List<System.Data.IDataParameter[]> cmdParmsList);
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
         IDataReader SP_PagingByReader(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where);
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
         DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where);

         /// <summary>
         /// 执行分页存储过程（带合计行）
         /// </summary>
         /// <param name="tableName">要查询的表</param>
         /// <param name="fields">要查询的字段</param>
         /// <param name="orderFields">用于排序的字段</param>
         /// <param name="id">查询记录中的唯一索引字段</param>
         /// <param name="pageIndex">页面索引</param>
         /// <param name="pageSize">页面大小</param>
         /// <param name="recordCount">总记录数</param>
         /// <param name="pageCount">页面总数</param>
         /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
         /// <param name="where">不要加where 关键字</param>
         /// <returns></returns>
         DataSet SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where, string totalFields);

         /// <summary>
         /// 执行分页存储过程(不带合计行)
         /// </summary>
         /// <param name="tableName">要查询的表</param>
         /// <param name="fields">要查询的字段</param>
         /// <param name="orderFields">用于排序的字段</param>
         /// <param name="id">查询记录中的唯一索引字段</param>
         /// <param name="pageIndex">页面索引</param>
         /// <param name="pageSize">页面大小</param>
         /// <param name="recordCount">总记录数</param>
         /// <param name="pageCount">页面总数</param>
         /// <param name="orderType">设置排序类型, 非 0 值则降序</param>
         /// <param name="where">不要加where 关键字</param>
         /// <returns></returns>
         DataTable SP_PageingByDataset(string tableName, string fields, string orderFields, string id, int pageIndex, int pageSize, ref int recordCount, ref int pageCount, int orderType, string where);
        
        /// <summary>
        /// 读取excel中的数据
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
         DataSet LoadDataFromExcel(string File, string sheetName);

         /// <summary>
         /// 使用SqlBulkCopy大批量插入数据
         /// </summary>
         /// <param name="tableName"></param>
         /// <param name="dt"></param>
         /// <returns></returns>
         bool AddDataBySqlBulkCopy(string tableName, DataTable dt);
    }
}
