using DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Model
{

    #region
    /// <summary>
    /// 反射的Sql语句缓存
    /// </summary>
    internal static class SqlCache
    {
        private static Dictionary<string, string> _SQLDictionary = new Dictionary<string, string>();
        /// <summary>
        /// 系统中所有反射生成的SQL语句缓存
        /// </summary>
        public static Dictionary<string, string> SQLDictionary
        {
            get { return SqlCache._SQLDictionary; }
            set { SqlCache._SQLDictionary = value; }
        }
    }
    internal interface IReflectionHelper
    {
        /// <summary>
        /// 生成insert语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateInsert(Type type);
        /// <summary>
        /// 生成根据主键删除语句 以主键为条件进行删除
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateDelete(Type type);
        /// <summary>
        /// 生成根据主键更新语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateUpdate(Type type);
        /// <summary>
        /// 生成更新语句,不带更新条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateUpdateWithoutWhere(Type type);
        /// <summary>
        /// 生成所有的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateFieldsName(Type type);
        /// <summary>
        /// 获取所有主键作为排序字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateOrderFieldsName(Type type);
        /// <summary>
        /// 根据主键查询的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateSelectByPrimeryKey(Type type);
        /// <summary>
        /// 获取该实体类对应的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string CreateTableName(Type type);
    }
    internal class Oracle2ReflectionHelper : IReflectionHelper
    {
        #region 对应ORACLE数据库
        /// <summary>
        /// 生成insert语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateInsert(Type type)
        {
            string typeName = string.Format("ORACLE_Insert_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder insert = new StringBuilder();

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                insert.AppendFormat("insert into \"{0}\" (", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        if (((DataEntityFieldAttribute)property_atts[0]).IsIdentity == false)
                        {
                            insert.AppendFormat("\"{0}\",", fieldName);
                        }
                    }
                }
                insert.Remove(insert.Length - 1, 1);
                insert.AppendFormat(") values (");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        if (((DataEntityFieldAttribute)property_atts[0]).IsIdentity == false)
                        {
                            insert.AppendFormat(":{0},", fieldName);
                        }
                    }
                }
                insert.Remove(insert.Length - 1, 1);
                insert.AppendFormat(")");

                SqlCache.SQLDictionary.Add(typeName, insert.ToString());

                return insert.ToString();
            }
        }
        /// <summary>
        /// 生成根据主键删除语句 以主键为条件进行删除
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateDelete(Type type)
        {
            string typeName = string.Format("ORACLE_Delete_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder delete = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                delete.AppendFormat("delete from \"{0}\" where ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            delete.AppendFormat(" \"{0}\"=:{0} and", fieldName);
                        }

                    }
                }
                delete.Remove(delete.Length - 3, 3);

                SqlCache.SQLDictionary.Add(typeName, delete.ToString());

                return delete.ToString();
            }
        }
        /// <summary>
        /// 生成根据主键更新语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateUpdate(Type type)
        {
            string typeName = string.Format("ORACLE_Update_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder update = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                update.AppendFormat("update \"{0}\" set ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (!att.IsPrimaryKey && !att.IsIdentity)
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" \"{0}\"=:{0} ,", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 1, 1);
                update.AppendFormat(" where ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" \"{0}\"=:{0} and", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 3, 3);

                SqlCache.SQLDictionary.Add(typeName, update.ToString());

                return update.ToString();
            }
        }
        /// <summary>
        /// 生成更新语句,不带更新条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateUpdateWithoutWhere(Type type)
        {
            string typeName = string.Format("ORACLE_UpdateWithoutWhere_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder update = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                update.AppendFormat("update \"{0}\" set ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (!att.IsIdentity)//!att.IsPrimaryKey &&
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" \"{0}\"=:{0} ,", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, update.ToString());

                return update.ToString();
            }
        }
        /// <summary>
        /// 生成所有的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateFieldsName(Type type)
        {
            string typeName = string.Format("ORACLE_Fields_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder fields = new StringBuilder();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        fields.AppendFormat("\"{0}\",", fieldName);
                    }
                }
                fields.Remove(fields.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, fields.ToString());

                return fields.ToString();
            }
        }
        /// <summary>
        /// 获取所有主键作为排序字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateOrderFieldsName(Type type)
        {
            string typeName = string.Format("ORACLE_OrderFields_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder fields = new StringBuilder();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsIdentity || att.IsPrimaryKey)
                        {
                            fields.AppendFormat("\"{0}\",", att.FieldName);
                        }
                    }
                }
                if (fields.Length > 0)
                {
                    fields.Remove(fields.Length - 1, 1);
                }
                SqlCache.SQLDictionary.Add(typeName, fields.ToString());

                return fields.ToString();
            }
        }
        /// <summary>
        /// 根据主键查询的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateSelectByPrimeryKey(Type type)
        {
            string typeName = string.Format("ORACLE_Select_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder select = new StringBuilder();

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                select.AppendFormat("select ");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        select.AppendFormat("\"{0}\",", fieldName);
                    }
                }

                select.Remove(select.Length - 1, 1);
                select.AppendFormat(" from \"{0}\" ", tableName);


                select.AppendFormat(" where ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            select.AppendFormat(" \"{0}\"=:{1} ,", fieldName, fieldName);
                        }
                    }
                }
                select.Remove(select.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, select.ToString());

                return select.ToString();
            }
        }
        /// <summary>
        /// 获取该实体类对应的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateTableName(Type type)
        {
            string typeName = string.Format("ORACLE_TableName_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = string.Format("\"{0}\"", ((DataEntityAttribute)objs[0]).TableName);
                }

                SqlCache.SQLDictionary.Add(typeName, tableName);

                return tableName;
            }
        }
        #endregion


    }
    internal class SqlSerReflectionHelper : IReflectionHelper
    {
        #region 对应Sql Server数据库
        /// <summary>
        /// 生成insert语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateInsert(Type type)
        {
            string typeName = string.Format("SQLSERVER_Insert_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder insert = new StringBuilder();

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                insert.AppendFormat("insert into {0} (", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        if (((DataEntityFieldAttribute)property_atts[0]).IsIdentity == false)
                        {
                            insert.AppendFormat("{0},", fieldName);
                        }
                    }
                }
                insert.Remove(insert.Length - 1, 1);
                insert.AppendFormat(") values (");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        if (((DataEntityFieldAttribute)property_atts[0]).IsIdentity == false)
                        {
                            insert.AppendFormat("@{0},", fieldName);
                        }
                    }
                }
                insert.Remove(insert.Length - 1, 1);
                insert.AppendFormat(")");

                SqlCache.SQLDictionary.Add(typeName, insert.ToString());

                return insert.ToString();
            }
        }
        /// <summary>
        /// 生成根据主键删除语句 以主键为条件进行删除
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateDelete(Type type)
        {
            string typeName = string.Format("SQLSERVER_Delete_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder delete = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                delete.AppendFormat("delete from {0} where ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            delete.AppendFormat(" {0}=@{0} and", fieldName);
                        }

                    }
                }
                delete.Remove(delete.Length - 3, 3);

                SqlCache.SQLDictionary.Add(typeName, delete.ToString());

                return delete.ToString();
            }
        }
        /// <summary>
        /// 生成根据主键更新语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateUpdate(Type type)
        {
            string typeName = string.Format("SQLSERVER_Update_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder update = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                update.AppendFormat("update {0} set ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (!att.IsPrimaryKey && !att.IsIdentity)
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" {0}=@{0} ,", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 1, 1);
                update.AppendFormat(" where ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" {0}=@{0} and", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 3, 3);

                SqlCache.SQLDictionary.Add(typeName, update.ToString());

                return update.ToString();
            }
        }
        /// <summary>
        /// 生成更新语句,不带更新条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateUpdateWithoutWhere(Type type)
        {
            string typeName = string.Format("SQLSERVER_UpdateWithoutWhere_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder update = new StringBuilder();
                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                update.AppendFormat("update {0} set ", tableName);

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (!att.IsIdentity)//!att.IsPrimaryKey &&
                        {
                            string fieldName = att.FieldName;
                            update.AppendFormat(" {0}=@{0} ,", fieldName);
                        }
                    }
                }
                update.Remove(update.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, update.ToString());

                return update.ToString();
            }
        }
        /// <summary>
        /// 生成所有的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateFieldsName(Type type)
        {
            string typeName = string.Format("SQLSERVER_Fields_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder fields = new StringBuilder();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        fields.AppendFormat("{0},", fieldName);
                    }
                }
                fields.Remove(fields.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, fields.ToString());

                return fields.ToString();
            }
        }
        /// <summary>
        /// 获取所有主键作为排序字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateOrderFieldsName(Type type)
        {
            string typeName = string.Format("SQLSERVER_OrderFields_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder fields = new StringBuilder();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsIdentity || att.IsPrimaryKey)
                        {
                            fields.AppendFormat("{0},", att.FieldName);
                        }
                    }
                }
                if (fields.Length > 0)
                {
                    fields.Remove(fields.Length - 1, 1);
                }
                SqlCache.SQLDictionary.Add(typeName, fields.ToString());

                return fields.ToString();
            }
        }
        /// <summary>
        /// 根据主键查询的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateSelectByPrimeryKey(Type type)
        {
            string typeName = string.Format("SQLSERVER_Select_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {
                StringBuilder select = new StringBuilder();

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }
                select.AppendFormat("select ");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;
                        select.AppendFormat("{0},", fieldName);
                    }
                }

                select.Remove(select.Length - 1, 1);
                select.AppendFormat(" from {0} ", tableName);


                select.AppendFormat(" where ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                        if (att.IsPrimaryKey)
                        {
                            string fieldName = att.FieldName;
                            select.AppendFormat(" {0}=@{1} ,", fieldName, fieldName);
                        }
                    }
                }
                select.Remove(select.Length - 1, 1);

                SqlCache.SQLDictionary.Add(typeName, select.ToString());

                return select.ToString();
            }
        }
        /// <summary>
        /// 获取该实体类对应的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CreateTableName(Type type)
        {
            string typeName = string.Format("SQLSERVER_TableName_{0}", type.FullName);
            if (SqlCache.SQLDictionary.Keys.Contains(typeName))
            {
                return SqlCache.SQLDictionary[typeName];
            }
            else
            {

                object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
                string tableName = "";
                if (objs.Length > 0)
                {
                    tableName = ((DataEntityAttribute)objs[0]).TableName;
                }

                SqlCache.SQLDictionary.Add(typeName, tableName);

                return tableName;
            }
        }
        #endregion
    }
    #endregion
    /// <summary>
    /// 通过对数据实体类的反射生成SQL
    /// </summary>
    public static class ReflectionHelper
    {
        private static IReflectionHelper helper = null;
        private static MyDbType dbType = MyDbType.None;
        static ReflectionHelper()
        {
            dbType = DBHelper.MyDBHelper.DbType;
            switch (dbType)
            {
                case MyDbType.None:
                    break;
                case MyDbType.Oracle:
                    helper = new Oracle2ReflectionHelper();
                    break;
                case MyDbType.SqlServer:
                    helper = new SqlSerReflectionHelper();
                    break;
            }
        }

        /// <summary>
        /// 生成insert语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateInsert(Type type)
        {
            return helper.CreateInsert(type);
        }
        /// <summary>
        /// 生成根据主键删除语句 以主键为条件进行删除
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateDelete(Type type)
        {
            return helper.CreateDelete(type);
        }
        /// <summary>
        /// 生成根据主键更新语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateUpdate(Type type)
        {
            return helper.CreateUpdate(type);
        }
        /// <summary>
        /// 生成更新语句,不带更新条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateUpdateWithoutWhere(Type type)
        {
            return helper.CreateUpdateWithoutWhere(type);
        }
        /// <summary>
        /// 生成所有的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateFieldsName(Type type)
        {
            return helper.CreateFieldsName(type);
        }
        /// <summary>
        /// 获取所有主键作为排序字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateOrderFieldsName(Type type)
        {
            return helper.CreateOrderFieldsName(type);
        }
        /// <summary>
        /// 根据主键查询的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateSelectByPrimeryKey(Type type)
        {
            return helper.CreateSelectByPrimeryKey(type);
        }
        /// <summary>
        /// 获取该实体类对应的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CreateTableName(Type type)
        {
            return helper.CreateTableName(type);
        }
    }
}
