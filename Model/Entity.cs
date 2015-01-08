using DBHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Windows.Input;
using System.Runtime.Serialization;

namespace DBModel
{
    /// <summary>
    /// 数据库实体类的基类
    /// </summary>
    [DataContractAttribute]
    public class Entity : INotifyPropertyChanged
    {
        #region
        /// <summary>
        /// 选中事件
        /// </summary>
        public event EventHandler SelectedEvnetHandler; 
        public event PropertyChangedEventHandler PropertyChanged;
        protected void changed(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Entity()
        {
            OnClick = new EntityEventCommand(delegate()
            {
                //if (isSelected)
                //{
                //    IsSelected = false;
                //}
                //else
                //{
                //    IsSelected = true;
                //}
                if (SelectedEvnetHandler != null)
                {
                    SelectedEvnetHandler(this, new EventArgs());
                }
            });
        }
        public ICommand OnClick { get; set; }
        private bool isSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; changed("IsSelected"); }
        }
        #endregion
        /// <summary>
        /// 查询条件
        /// </summary>
        private string whereStr;
        [JsonIgnore]
        internal string WhereStr
        {
            get { return whereStr; }
            set { whereStr = value; }
        }
        private string limit = "";
        /// <summary>
        /// 查询限制条数
        /// </summary>
        [JsonIgnore]
        internal string Limit
        {
            get { return limit; }
            set { limit = value; }
        }
        private List<IDataParameter> parameters = new List<IDataParameter>();
        /// <summary>
        /// 查询参数定义
        /// </summary>
        [JsonIgnore]
        public List<IDataParameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
        private int rowSort = 0;
        /// <summary>
        /// 数据的排序
        /// </summary>
        public int RowSort
        {
            get { return rowSort; }
            set { rowSort = value; }
        }
        /// <summary>
        /// <para>用于校验提交的数据的完整性</para>
        /// <para>可以通过重写该函数，实现自定义的校验方式</para>
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {

            StringBuilder msg = new StringBuilder();
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                object[] property_atts = pi.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (property_atts.Length > 0)
                {
                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                    if (!att.IsIdentity)
                    {
                        object value = pi.GetValue(this, null);
                        if (value != null && value.GetType() == typeof(string))
                        {
                            pi.SetValue(this, value.ToString().Trim(), null);
                        }
                        if (att.IsPrimaryKey)
                        {
                            if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
                            {
                                msg.AppendFormat("{0}的值不能为空\r\n", att.FieldRemark);
                            }
                        }
                        else
                        {
                            if (!att.AllowNull)
                            {
                                if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
                                {
                                    msg.AppendFormat("{0}的值不能为空，", att.FieldRemark);
                                }
                            }
                        }
                    }
                }
            }
            return msg.ToString();
        }
    }

    public class EntityEventCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action action;

        public EntityEventCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
    public static class EntityExpand
    {

        /// <summary>
        /// 实体类序列化为指定格式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ToSerialization(this Entity entity)
        {
            StringBuilder str = new StringBuilder();
            foreach (PropertyInfo pi in entity.GetType().GetProperties())
            {
                object[] property_atts = pi.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (property_atts.Length > 0)
                {
                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
                    str.AppendFormat("[{0}|{1}:{2}]", att.FieldRemark == "" ? att.FieldName : att.FieldRemark, att.FieldRemark, pi.GetValue(entity, null));
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// 设置查询返回数据的条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="count"></param>
        public static void Top<T>(this T obj, int count) where T : Entity
        {
            if (count > 1)
            {
                obj.Limit = string.Format(" TOP {0}", count);
            }
        }
        public static bool In<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool NotIn<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool Like(this string str, string likeStr)
        {
            return true;
        }
        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
        public static string Left(this string str, int length)
        {
            return string.Empty;
        }
        public static string Right(this string str, int length)
        {
            return string.Empty;
        }
        /// <summary>
        /// 设置查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="func"></param>
        public static T Where<T>(this T entity, Expression<Func<T, bool>> func) where T : Entity
        {
            entity.WhereStr = string.Empty;
            entity.Parameters.Clear();

            switch (MyDBHelper.DbType)
            {
                case MyDbType.SqlServer:
                    if (func.Body is BinaryExpression)
                    {
                        BinaryExpression be = ((BinaryExpression)func.Body);
                        entity.WhereStr = SQL_BinaryExpressionProvider(be.Left, be.Right, be.NodeType, entity);
                        SQL_ParameterProvider(be.Left, be.Right, entity);
                    }
                    else
                    {
                        entity.WhereStr = SQL_ExpressionRouter(func.Body, entity);
                    }
                    break;
            }
            entity.Parameters = entity.Parameters.MyDistinct(p => p.ParameterName).ToList();
            return entity;
        }

        #region 针对sql的转换
        static string SQL_BinaryExpressionProvider<T>(Expression left, Expression right, ExpressionType type, T entity) where T : Entity
        {
            string sb = "(";
            //先处理左边
            sb += SQL_ExpressionRouter(left, entity);

            sb += SQL_ExpressionTypeCast(type);

            //再处理右边
            string tmpStr = "";
            if (right is MemberExpression || right is UnaryExpression)
            {
                tmpStr = "@" + SQL_ExpressionRouter(right, entity);
            }
            else
            {
                tmpStr = SQL_ExpressionRouter(right, entity);
            }

            if (tmpStr == "null")
            {
                if (sb.EndsWith(" = "))
                    sb = sb.Substring(0, sb.Length - 2) + " is null";
                else if (sb.EndsWith(" <> "))
                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
            }
            else
                sb += tmpStr;
            return sb += ")";
        }
        /// <summary>
        /// 分析表达式，将表达式转为SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        static string SQL_ExpressionRouter<T>(Expression exp, T entity) where T : Entity
        {
            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return SQL_BinaryExpressionProvider(be.Left, be.Right, be.NodeType, entity);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                object[] atts = me.Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (atts.Length > 0)
                {

                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                    return att.FieldName;
                }
                return me.Member.Name;
            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(SQL_ExpressionRouter(ex, entity));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                #region
                MethodCallExpression mce = (MethodCallExpression)exp;
                if (mce.Method.Name == "Like")
                {
                    #region like 查询
                    string fileName = SQL_ExpressionRouter(mce.Arguments[0], entity);
                    string paraName = SQL_ExpressionRouter(mce.Arguments[1], entity);

                    if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
                    {
                        object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                        if (atts.Length > 0)
                        {
                            SqlParameter para = new SqlParameter();
                            DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                            para.ParameterName = string.Format("@{0}", paraName);
                            para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);

                            Expression exp1 = mce.Arguments[1].ReduceExtensions();
                            var objectMember = Expression.Convert(exp1, typeof(object));
                            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                            var getter = getterLambda.Compile();
                            var value = getter();
                            para.Value = "%" + value + "%";

                            //Expression exp1 = mce.Arguments[1].ReduceExtensions();
                            //MemberExpression r_me = (MemberExpression)exp1;
                            //ConstantExpression ce = (ConstantExpression)r_me.Expression;
                            //var pi = ce.Value.GetType().GetField(r_me.Member.Name);
                            //para.Value = "%" + pi.GetValue(ce.Value).ToString() + "%";
                            entity.Parameters.Add(para);

                            return string.Format(" {0} like @{1} ", fileName, paraName);
                        }
                    }
                    if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is ConstantExpression)
                    {
                        //object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                        //if (atts.Length > 0)
                        //{
                        //    SqlParameter para = new SqlParameter();
                        //    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        //    para.ParameterName = string.Format("@{0}", paraName);
                        //    para.SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), att.FieldType, true);
                        //    Expression exp1 = mce.Arguments[1].ReduceExtensions();
                        //    MemberExpression r_me = (MemberExpression)exp1;
                        //    ConstantExpression ce = (ConstantExpression)r_me.Expression;
                        //    var pi = ce.Value.GetType().GetField(r_me.Member.Name);
                        //    para.Value = "%" + pi.GetValue(ce.Value).ToString() + "%";
                        //    entity.Parameters.Add(para);
                        //}
                        return string.Format(" {0} like '%{1}%' ", fileName, paraName);
                    }
                    return string.Format(" {0} like {1} ", fileName, paraName);

                    #endregion
                }
                else if (mce.Method.Name == "NotLike")
                {
                    #region NotLike查询
                    string fileName = SQL_ExpressionRouter(mce.Arguments[0], entity);
                    string paraName = SQL_ExpressionRouter(mce.Arguments[1], entity);

                    if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
                    {
                        object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                        if (atts.Length > 0)
                        {
                            SqlParameter para = new SqlParameter();
                            DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                            para.ParameterName = string.Format("@{0}", paraName);
                            para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);

                            Expression exp1 = mce.Arguments[1].ReduceExtensions();
                            var objectMember = Expression.Convert(exp1, typeof(object));
                            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                            var getter = getterLambda.Compile();
                            var value = getter();
                            para.Value = "%" + value + "%";

                            //Expression exp1 = mce.Arguments[1].ReduceExtensions();
                            //MemberExpression r_me = (MemberExpression)exp1;
                            //ConstantExpression ce = (ConstantExpression)r_me.Expression;
                            //var pi = ce.Value.GetType().GetField(r_me.Member.Name);
                            //para.Value = "%" + pi.GetValue(ce.Value).ToString() + "%";

                            entity.Parameters.Add(para);
                        }
                    }

                    return string.Format("({0} Not like @{1})", fileName, paraName);
                    #endregion
                }
                else if (mce.Method.Name == "In")
                {
                    #region In查询

                    //string fileName = SQL_ExpressionRouter(mce.Arguments[0], entity);
                    //string paraName = SQL_ExpressionRouter(mce.Arguments[1], entity);
                    //string whereParam = "";

                    //if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
                    //{
                    //    object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    //    if (atts.Length > 0)
                    //    {
                    //        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                    //        Expression exp1 = mce.Arguments[1].ReduceExtensions();
                    //        MemberExpression r_me = (MemberExpression)exp1;
                    //        ConstantExpression ce = (ConstantExpression)r_me.Expression;
                    //        FieldInfo pi = ce.Value.GetType().GetField(r_me.Member.Name);
                    //        if (pi.FieldType.FullName == "System.String[]")
                    //        {
                    //            string[] array = (string[])pi.GetValue(ce.Value);
                    //            for (int i = 0; i < array.Length; i++)
                    //            {
                    //                string str = array[i];
                    //                SqlParameter para = new SqlParameter();
                    //                para.ParameterName = string.Format("@{0}{1}", paraName, i);
                    //                para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                    //                para.Value = str;
                    //                whereParam += string.Format(",@{0}{1}", paraName, i);
                    //                entity.Parameters.Add(para);
                    //            }
                    //            if (whereParam.Length > 0)
                    //            {
                    //                whereParam = whereParam.Substring(1);
                    //            }
                    //        }
                    //    }
                    //}
                    //return string.Format("{0} In ({1})", SQL_ExpressionRouter(mce.Arguments[0], entity), whereParam);

                    //将参数化的查询方式改为拼接字符串的方式

                    string fileName = SQL_ExpressionRouter(mce.Arguments[0], entity);
                    string paraName = SQL_ExpressionRouter(mce.Arguments[1], entity);
                    string whereParam = "";

                    if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
                    {
                        object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                        if (atts.Length > 0)
                        {
                            DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                            Expression exp1 = mce.Arguments[1].ReduceExtensions();
                            MemberExpression r_me = (MemberExpression)exp1;
                            ConstantExpression ce = (ConstantExpression)r_me.Expression;
                            FieldInfo pi = ce.Value.GetType().GetField(r_me.Member.Name);

                            if (pi.FieldType.FullName == "System.String[]")
                            {
                                string[] array = (string[])pi.GetValue(ce.Value);
                                for (int i = 0; i < array.Length; i++)
                                {
                                    whereParam += string.Format(",'{0}'", array[i]);
                                }
                                if (whereParam.Length > 0)
                                {
                                    whereParam = whereParam.Substring(1);
                                }
                            }
                            else if (pi.FieldType.FullName == "System.Decimal[]")
                            {
                                System.Decimal[] array = (System.Decimal[])pi.GetValue(ce.Value);
                                for (int i = 0; i < array.Length; i++)
                                {
                                    whereParam += string.Format(",{0}", array[i]);
                                }
                                if (whereParam.Length > 0)
                                {
                                    whereParam = whereParam.Substring(1);
                                }
                            }
                            else if (pi.FieldType.FullName == "System.Int32[]")
                            {
                                System.Int32[] array = (System.Int32[])pi.GetValue(ce.Value);
                                for (int i = 0; i < array.Length; i++)
                                {
                                    whereParam += string.Format(",{0}", array[i]);
                                }
                                if (whereParam.Length > 0)
                                {
                                    whereParam = whereParam.Substring(1);
                                }
                            }
                        }
                    }
                    return string.Format("{0} In ({1})", SQL_ExpressionRouter(mce.Arguments[0], entity), whereParam);
                    #endregion
                }
                else if (mce.Method.Name == "NotIn")
                {
                    #region NotIn查询
                    return string.Format("{0} Not In ({1})", SQL_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
                    #endregion
                }
                else if (mce.Method.Name == "Left")
                {
                    return string.Format(" left(@{0},{1})", SQL_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
                }
                else if (mce.Method.Name == "Right")
                {
                    return string.Format(" right(@{0},{1})", SQL_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
                }
                #endregion
            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is Boolean)
                    return (bool)ce.Value ? "1" : "0";
                else if (ce.Value is ValueType)
                    return ce.Value.ToString();
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                    return string.Format("'{0}'", ce.Value.ToString());
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return SQL_ExpressionRouter(ue.Operand, entity);
            }
            return null;
        }
        /// <summary>
        /// 解析操作符
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string SQL_ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";
                default:
                    return null;
            }
        }
        static void SQL_ParameterProvider(Expression left, Expression right, Entity entity)
        {
            SqlParameter para = new SqlParameter();

            if (left is BinaryExpression || right is BinaryExpression)
            {
                SQL_ParameterValue(left, entity);
                SQL_ParameterValue(right, entity);
                return;
            }
            if (left is MethodCallExpression)
            {
                SQL_ParameterValue(right, entity);
                return;
            }
            if (right is MethodCallExpression)
            {
                SQL_ParameterValue(left, entity);
                return;
            }
            //根据左边表达式获取参数的类型
            MemberExpression l_me = ((MemberExpression)left);
            object[] atts = l_me.Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
            //获取参数类型
            if (atts.Length > 0)
            {
                DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
            }
            else
            {
                return;
            }
            //根据右边表达式获取参数名称
            if (right is MemberExpression || right is UnaryExpression)
            {
                para.ParameterName = string.Format("@{0}", SQL_ExpressionRouter(right, entity));
            }
            //获取参数的值
            if (right is MemberExpression)
            {
                if (((MemberExpression)right).Expression is ConstantExpression)
                {
                    var objectMember = Expression.Convert(right, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var value = getter();
                    para.Value = value;
                }
                else
                {
                    var objectMember = Expression.Convert(right, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var value = getter();
                    para.Value = value;
                }
                entity.Parameters.Add(para);
            }
            else if (right is UnaryExpression || right is MethodCallExpression)
            {
                var objectMember = Expression.Convert(right, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                var value = getter();
                para.Value = value;
                entity.Parameters.Add(para);
            }
            else
            {
                SQL_ParameterValue(left, entity);
                SQL_ParameterValue(right, entity);
            }

        }
        static void SQL_ParameterValue(Expression exp, Entity entity)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                SQL_ParameterProvider(be.Left, be.Right, entity);
            }
        }
        #endregion

        //#region 针对oracle的转换
        //static string ORACLE_BinaryExpressionProvider<T>(Expression left, Expression right, ExpressionType type, T entity) where T : Entity
        //{
        //    string sb = "(";
        //    //先处理左边
        //    sb += ORACLE_ExpressionRouter(left, entity);

        //    sb += ORACLE_ExpressionTypeCast(type);

        //    //再处理右边
        //    string tmpStr = "";
        //    if (right is MemberExpression || right is UnaryExpression)
        //    {
        //        tmpStr = ":" + ORACLE_ExpressionRouter(right, entity);
        //    }
        //    else
        //    {
        //        tmpStr = ORACLE_ExpressionRouter(right, entity);
        //    }

        //    if (tmpStr == "null")
        //    {
        //        if (sb.EndsWith(" = "))
        //            sb = sb.Substring(0, sb.Length - 2) + " is null";
        //        else if (sb.EndsWith(" <> "))
        //            sb = sb.Substring(0, sb.Length - 2) + " is not null";
        //    }
        //    else
        //        sb += tmpStr;
        //    return sb += ")";
        //}
        ///// <summary>
        ///// 分析表达式，将表达式转为SQL
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="exp"></param>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //static string ORACLE_ExpressionRouter<T>(Expression exp, T entity) where T : Entity
        //{
        //    string sb = string.Empty;
        //    if (exp is BinaryExpression)
        //    {
        //        BinaryExpression be = ((BinaryExpression)exp);
        //        return ORACLE_BinaryExpressionProvider(be.Left, be.Right, be.NodeType, entity);
        //    }
        //    else if (exp is MemberExpression)
        //    {
        //        MemberExpression me = ((MemberExpression)exp);
        //        object[] atts = me.Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
        //        if (atts.Length > 0)
        //        {

        //            DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
        //            return "\"" + att.FieldName + "\"";
        //        }
        //        return me.Member.Name;
        //    }
        //    else if (exp is NewArrayExpression)
        //    {
        //        NewArrayExpression ae = ((NewArrayExpression)exp);
        //        StringBuilder tmpstr = new StringBuilder();
        //        foreach (Expression ex in ae.Expressions)
        //        {
        //            tmpstr.Append(ORACLE_ExpressionRouter(ex, entity));
        //            tmpstr.Append(",");
        //        }
        //        return tmpstr.ToString(0, tmpstr.Length - 1);
        //    }
        //    else if (exp is MethodCallExpression)
        //    {
        //        #region
        //        MethodCallExpression mce = (MethodCallExpression)exp;
        //        if (mce.Method.Name == "Like")
        //        {
        //            string fileName = ORACLE_ExpressionRouter(mce.Arguments[0], entity);
        //            string paraName = ORACLE_ExpressionRouter(mce.Arguments[1], entity);

        //            if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
        //            {
        //                object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
        //                if (atts.Length > 0)
        //                {
        //                    OracleParameter para = new OracleParameter();
        //                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
        //                    para.ParameterName = string.Format(":{0}", paraName);
        //                    para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
        //                    Expression exp1 = mce.Arguments[1].ReduceExtensions();
        //                    MemberExpression r_me = (MemberExpression)exp1;
        //                    ConstantExpression ce = (ConstantExpression)r_me.Expression;
        //                    var pi = ce.Value.GetType().GetField(r_me.Member.Name);
        //                    para.Value = "%" + pi.GetValue(ce.Value).ToString() + "%";
        //                    entity.Parameters.Add(para);

        //                    return string.Format(" {0} like :{1} ", fileName, paraName);
        //                }
        //            }
        //            if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is ConstantExpression)
        //            {

        //                return string.Format(" {0} like '%{1}%' ", fileName, paraName);
        //            }
        //            return string.Format(" {0} like {1} ", fileName, paraName);
        //        }
        //        else if (mce.Method.Name == "NotLike")
        //        {
        //            string fileName = ORACLE_ExpressionRouter(mce.Arguments[0], entity);
        //            string paraName = ORACLE_ExpressionRouter(mce.Arguments[1], entity);

        //            if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
        //            {
        //                object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
        //                if (atts.Length > 0)
        //                {
        //                    OracleParameter para = new OracleParameter();
        //                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
        //                    para.ParameterName = string.Format(":{0}", paraName);
        //                    para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
        //                    Expression exp1 = mce.Arguments[1].ReduceExtensions();
        //                    MemberExpression r_me = (MemberExpression)exp1;
        //                    ConstantExpression ce = (ConstantExpression)r_me.Expression;
        //                    var pi = ce.Value.GetType().GetField(r_me.Member.Name);
        //                    para.Value = "%" + pi.GetValue(ce.Value).ToString() + "%";
        //                    entity.Parameters.Add(para);
        //                }
        //            }

        //            return string.Format("({0} Not like :{1})", fileName, paraName);
        //        }
        //        else if (mce.Method.Name == "In")
        //        {
        //            string fileName = ORACLE_ExpressionRouter(mce.Arguments[0], entity);
        //            string paraName = ORACLE_ExpressionRouter(mce.Arguments[1], entity);
        //            string whereParam = "";

        //            if (mce.Arguments[0] is MemberExpression && mce.Arguments[1] is MemberExpression)
        //            {
        //                object[] atts = ((MemberExpression)mce.Arguments[0]).Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
        //                if (atts.Length > 0)
        //                {
        //                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
        //                    Expression exp1 = mce.Arguments[1].ReduceExtensions();
        //                    MemberExpression r_me = (MemberExpression)exp1;
        //                    ConstantExpression ce = (ConstantExpression)r_me.Expression;
        //                    FieldInfo pi = ce.Value.GetType().GetField(r_me.Member.Name);
        //                    if (pi.FieldType.FullName == "System.String[]")
        //                    {
        //                        string[] array = (string[])pi.GetValue(ce.Value);
        //                        for (int i = 0; i < array.Length; i++)
        //                        {
        //                            string str = array[i];
        //                            OracleParameter para = new OracleParameter();
        //                            para.ParameterName = string.Format(":{0}{1}", paraName, i);
        //                            para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
        //                            para.Value = str;
        //                            whereParam += string.Format(",:{0}{1}", paraName, i);
        //                            entity.Parameters.Add(para);
        //                        }
        //                        if (whereParam.Length > 0)
        //                        {
        //                            whereParam = whereParam.Substring(1);
        //                        }
        //                    }
        //                }
        //            }
        //            return string.Format("{0} In ({1})", ORACLE_ExpressionRouter(mce.Arguments[0], entity), whereParam);
        //        }
        //        else if (mce.Method.Name == "NotIn")
        //        {
        //            return string.Format("{0} Not In ({1})", ORACLE_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
        //        }
        //        else if (mce.Method.Name == "Left")
        //        {
        //            return string.Format(" left(:{0},{1})", ORACLE_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
        //        }
        //        else if (mce.Method.Name == "Right")
        //        {
        //            return string.Format(" right(:{0},{1})", ORACLE_ExpressionRouter(mce.Arguments[0], entity), SQL_ExpressionRouter(mce.Arguments[1], entity), SQL_ExpressionRouter(mce.Arguments[1], entity));
        //        }
        //        #endregion
        //    }
        //    else if (exp is ConstantExpression)
        //    {
        //        ConstantExpression ce = ((ConstantExpression)exp);
        //        if (ce.Value == null)
        //            return "null";
        //        else if (ce.Value is Boolean)
        //            return (bool)ce.Value ? "1" : "0";
        //        else if (ce.Value is ValueType)
        //            return ce.Value.ToString();
        //        else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
        //            return string.Format("'{0}'", ce.Value.ToString());
        //    }
        //    else if (exp is UnaryExpression)
        //    {
        //        UnaryExpression ue = ((UnaryExpression)exp);
        //        return ORACLE_ExpressionRouter(ue.Operand, entity);
        //    }
        //    return null;
        //}
        ///// <summary>
        ///// 解析操作符
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //static string ORACLE_ExpressionTypeCast(ExpressionType type)
        //{
        //    switch (type)
        //    {
        //        case ExpressionType.And:
        //        case ExpressionType.AndAlso:
        //            return " AND ";
        //        case ExpressionType.Equal:
        //            return " = ";
        //        case ExpressionType.GreaterThan:
        //            return " > ";
        //        case ExpressionType.GreaterThanOrEqual:
        //            return " >= ";
        //        case ExpressionType.LessThan:
        //            return " < ";
        //        case ExpressionType.LessThanOrEqual:
        //            return " <= ";
        //        case ExpressionType.NotEqual:
        //            return " <> ";
        //        case ExpressionType.Or:
        //        case ExpressionType.OrElse:
        //            return " Or ";
        //        case ExpressionType.Add:
        //        case ExpressionType.AddChecked:
        //            return " + ";
        //        case ExpressionType.Subtract:
        //        case ExpressionType.SubtractChecked:
        //            return " - ";
        //        case ExpressionType.Divide:
        //            return " / ";
        //        case ExpressionType.Multiply:
        //        case ExpressionType.MultiplyChecked:
        //            return " * ";
        //        default:
        //            return null;
        //    }
        //}
        //static void ORACLE_ParameterProvider(Expression left, Expression right, Entity entity)
        //{
        //    OracleParameter para = new OracleParameter();

        //    if (left is BinaryExpression || right is BinaryExpression)
        //    {
        //        ORACLE_ParameterValue(left, entity);
        //        ORACLE_ParameterValue(right, entity);
        //        return;
        //    }
        //    if (left is MethodCallExpression)
        //    {
        //        ORACLE_ParameterValue(right, entity);
        //        return;
        //    }
        //    if (right is MethodCallExpression)
        //    {
        //        ORACLE_ParameterValue(left, entity);
        //        return;
        //    }
        //    //根据左边表达式获取参数的类型
        //    MemberExpression l_me = ((MemberExpression)left);
        //    object[] atts = l_me.Member.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
        //    //获取参数类型
        //    if (atts.Length > 0)
        //    {
        //        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
        //        para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    //根据右边表达式获取参数名称
        //    if (right is MemberExpression || right is UnaryExpression)
        //    {
        //        para.ParameterName = string.Format(":{0}", ORACLE_ExpressionRouter(right, entity));
        //    }
        //    //获取参数的值
        //    if (right is MemberExpression)
        //    {
        //        if (((MemberExpression)right).Expression is ConstantExpression)
        //        {
        //            var objectMember = Expression.Convert(right, typeof(object));
        //            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        //            var getter = getterLambda.Compile();
        //            var value = getter();
        //            para.Value = value;
        //            //MemberExpression r_me = (MemberExpression)right;
        //            //ConstantExpression ce = (ConstantExpression)r_me.Expression;
        //            //var pi = ce.Value.GetType().GetField(r_me.Member.Name);
        //            //para.Value = pi.GetValue(ce.Value);
        //        }
        //        else
        //        {
        //            var objectMember = Expression.Convert(right, typeof(object));
        //            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        //            var getter = getterLambda.Compile();
        //            var value = getter();
        //            para.Value = value;
        //        }
        //        entity.Parameters.Add(para);
        //    }
        //    else if (right is UnaryExpression || right is MethodCallExpression)
        //    {
        //        var objectMember = Expression.Convert(right, typeof(object));
        //        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        //        var getter = getterLambda.Compile();
        //        var value = getter();
        //        para.Value = value;
        //        entity.Parameters.Add(para);
        //    }
        //    else
        //    {
        //        SQL_ParameterValue(left, entity);
        //        SQL_ParameterValue(right, entity);
        //    }

        //}
        //static void ORACLE_ParameterValue(Expression exp, Entity entity)
        //{
        //    if (exp is BinaryExpression)
        //    {
        //        BinaryExpression be = ((BinaryExpression)exp);
        //        ORACLE_ParameterProvider(be.Left, be.Right, entity);
        //    }
        //}
        //#endregion


    }

    public static class EntityListExpand
    {
        /// <summary>
        /// 将实体List转为Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToJosn<T>(this List<T> list) where T : Entity
        {
            return Newtonsoft.Json.JavaScriptConvert.SerializeObject(list);
        }
        /// <summary>
        /// 将实体类转为json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ToJosn<T>(this T model) where T : Entity
        {
            return Newtonsoft.Json.JavaScriptConvert.SerializeObject(model);
        }
        /// <summary>
        /// 将实体List转为DataSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataSet ToDataSet<T>(this List<T> list) where T : Entity
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            Type type = typeof(T);
            object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
            string tableName = "";
            if (objs.Length > 0)
            {
                tableName = ((DataEntityAttribute)objs[0]).TableName;
                dt.Namespace = tableName;
            }

            foreach (PropertyInfo property in type.GetProperties())
            {
                object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (property_atts.Length > 0)
                {
                    string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;

                    dt.Columns.Add(new DataColumn(fieldName));
                }
            }

            foreach (T t in list)
            {
                DataRow row = dt.NewRow();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;

                        object value = property.GetValue(t, null);
                        row[fieldName] = value;
                    }
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds;
        }
        /// <summary>
        /// 将实体List转为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list) where T : Entity
        {
            DataTable dt = new DataTable();
            Type type = typeof(T);
            object[] objs = type.GetCustomAttributes(typeof(DataEntityAttribute), false);
            string tableName = "";
            if (objs.Length > 0)
            {
                tableName = ((DataEntityAttribute)objs[0]).TableName;
                dt.Namespace = tableName;
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (property_atts.Length > 0)
                {
                    string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;

                    dt.Columns.Add(new DataColumn(fieldName));
                }
            }

            foreach (T t in list)
            {
                DataRow row = dt.NewRow();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    object[] property_atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (property_atts.Length > 0)
                    {
                        string fieldName = ((DataEntityFieldAttribute)property_atts[0]).FieldName;

                        object value = property.GetValue(t, null);
                        row[fieldName] = value;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 将实体IEnumerable按指定的分隔符连接成一个字符串
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="list">泛型数组</param>
        /// <param name="delimiter">分隔符</param>
        /// <returns></returns>
        public static string ToJoin<T>(this IEnumerable<T> list, string delimiter)
        {
            StringBuilder str = new StringBuilder();
            foreach (T t in list)
            {
                str.AppendFormat("{0}{1}", delimiter, t);
            }
            if (str.Length > 0)
            {
                str.Remove(0, delimiter.Length);
            }
            return str.ToString();
        }
        /// <summary>
        /// 将实体IEnumerable按指定的分隔符连接成一个字符串
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="list">泛型数组</param>
        /// <param name="delimiter">分隔符</param>
        /// <param name="propertyName">泛型类的某一个字段</param>
        /// <returns></returns>
        public static string ToJoin<T>(this IEnumerable<T> list, string delimiter, Expression<Func<T, object>> propertyName)
        {
            StringBuilder str = new StringBuilder();
            foreach (T t in list)
            {
                var value = propertyName.Compile()(t);
                str.AppendFormat("{0}{1}", delimiter, value);
            }
            if (str.Length > 0)
            {
                str.Remove(0, delimiter.Length);
            }
            return str.ToString();
        }
        /// <summary>
        /// 讲datatable转为json格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable data)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");
            DataColumnCollection list = data.Columns;
            foreach (DataRow dr in data.Rows)
            {
                json.Append("{");
                foreach (DataColumn column in list)
                {
                    json.AppendFormat("\"{0}\":\"{1}\",", column.ColumnName, dr[column.ColumnName]);
                }
                json.Remove(json.Length - 1, 1);
                json.Append("},");
            }
            if (json.Length > 1)
            {
                json.Remove(json.Length - 1, 1);
            }
            json.Append("]");
            return json.ToString();
        }

        public static IEnumerable<T> MyDistinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }

        public static IEnumerable<T> MyDistinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector, comparer));
        }
    }

    public static class EntityDbExpand
    {
        #region 增删改查
        /// <summary>
        /// 分页查询（带合计行）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="page">页面索引</param>
        /// <param name="rows">页面大小</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序类型（0/1）</param>
        /// <param name="PrimaryKey">唯一索引字段</param>
        /// <returns></returns>
        public static PagerList<T> SelectPageList<T>(this T model, string page, string rows, string sort, string order, string PrimaryKey) where T : Entity
        {
            Type type = model.GetType();

            string tableName = ReflectionHelper.CreateTableName(type);
            string fields = ReflectionHelper.CreateFieldsName(type);
            string orderFields = string.IsNullOrEmpty(sort) ? ReflectionHelper.CreateOrderFieldsName(type) : sort;
            if (string.IsNullOrEmpty(orderFields))
            {
                string[] fieldList = fields.Split(',');
                orderFields = fieldList[0];
            }
            int orderType = order.Equals("desc", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            int pageIndex = string.IsNullOrEmpty(page) ? 1 : int.Parse(page);
            int pageSize = string.IsNullOrEmpty(rows) ? int.MaxValue : int.Parse(rows);
            string where = string.IsNullOrEmpty(model.WhereStr) ? "" : model.WhereStr;

            #region 要把entity中的参数化查询条件变成拼接字符串的查询条件
            foreach (SqlParameter para in model.Parameters)
            {
                if (para.SqlDbType == SqlDbType.Int ||
                    para.SqlDbType == SqlDbType.Decimal ||
                    para.SqlDbType == SqlDbType.Money ||
                    para.SqlDbType == SqlDbType.Float ||
                    para.SqlDbType == SqlDbType.NText
                    )
                {
                    where = where.Replace(para.ParameterName, para.Value.ToString());
                }
                else if (para.SqlDbType == SqlDbType.DateTime)
                {
                    where = where.Replace(para.ParameterName, "'" + para.Value.ToString() + "'");
                }
                else if (para.SqlDbType == SqlDbType.Bit)
                {
                    string v = (bool)para.Value ? "1" : "0";
                    where = where.Replace(para.ParameterName, v);
                }
                else
                {

                    where = where.Replace(para.ParameterName, string.Format("'{0}'", para.Value.ToString()));
                }
            }

            #endregion

            int recordCount = 0;
            int pageCount = 0;

            PropertyInfo[] p = typeof(T).GetProperties();
            //string totalFields = "";
            //foreach (PropertyInfo _p in p)
            //{
            //    object[] property_atts = _p.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
            //    if (property_atts.Length > 0)
            //    {
            //        DataEntityFieldAttribute att = (DataEntityFieldAttribute)property_atts[0];
            //        if (!att.IsIdentity && (_p.PropertyType == typeof(Int32) || _p.PropertyType == typeof(Decimal)))
            //        {
            //            totalFields += "," + _p.Name;
            //        }
            //    }
            //}
            //if (totalFields.Length > 0)
            //{
            //    totalFields = totalFields.Substring(1);
            //}

            DataSet ds = MyDBHelper.SP_PageingByDataset(tableName, fields, orderFields, PrimaryKey, pageIndex, pageSize, ref recordCount, ref pageCount, orderType, where, "");
            PagerList<T> DataList = new PagerList<T>();
            #region
            if (ds == null && ds.Tables.Count == 0 && ds.Tables[0].Rows.Count == 0)
            {
                return DataList;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                T RowInstance = Activator.CreateInstance<T>();
                foreach (PropertyInfo pro in p)
                {
                    if (pro.IsDefined(typeof(DataEntityFieldAttribute), false))
                    {
                        try
                        {
                            if (pro.IsDefined(typeof(DataEntityFieldAttribute), false))
                            {
                                var obj = dr[pro.Name];
                                if (obj != DBNull.Value)
                                {
                                    pro.SetValue(RowInstance, obj, null);
                                }
                            }
                        }
                        catch { }
                    }
                }
                DataList.Add(RowInstance);
            }
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[1].Rows[0];
                T RowInstance = Activator.CreateInstance<T>();
                foreach (PropertyInfo pro in p)
                {
                    if (pro.IsDefined(typeof(DataEntityFieldAttribute), false) && pro.PropertyType == typeof(Int32) || pro.PropertyType == typeof(Decimal))
                    {
                        try
                        {
                            if (pro.IsDefined(typeof(DataEntityFieldAttribute), false))
                            {
                                var obj = dr[pro.Name];
                                if (obj != DBNull.Value)
                                {
                                    pro.SetValue(RowInstance, obj, null);
                                }
                            }
                        }
                        catch { }
                    }
                }
                DataList.Foot = RowInstance;
            }
            #endregion
            DataList.Total = recordCount;
            DataList.PageCount = pageCount;
            return DataList;
        }

        /// <summary>
        /// 根据查询条件获取多个实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> SelectList<T>(this T model) where T : Entity
        {
            if (model != null && (string.IsNullOrEmpty(model.WhereStr) == false || model.Parameters.Count > 0))
            {
                string sql = "select {0} {1} from {2} where {3}";
                sql = string.Format(sql, model.Limit, ReflectionHelper.CreateFieldsName(model.GetType()), ReflectionHelper.CreateTableName(model.GetType()), model.WhereStr);
                IDataReader reader = MyDBHelper.QueryReader(sql, model.Parameters.ToArray());
                return FillDataListGeneric<T>(reader);
            }
            else
            {
                T RowInstance = Activator.CreateInstance<T>();
                string sql = "select {0} {1} from {2} ";
                sql = string.Format(sql, model.Limit, ReflectionHelper.CreateFieldsName(RowInstance.GetType()), ReflectionHelper.CreateTableName(RowInstance.GetType()));
                IDataReader reader = MyDBHelper.QueryReader(sql);
                return FillDataListGeneric<T>(reader);
            }
        }
        /// <summary>
        /// 根据查询条件获取多个实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static List<T> SelectList<T>(this T model, Expression<Func<T, bool>> where) where T : Entity
        {
            model = model.Where(where);
            return model.SelectList();
        }
        /// <summary>
        /// 根据查询条件获取一个实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Select<T>(this T model) where T : Entity
        {
            if (string.IsNullOrEmpty(model.WhereStr))
            {
                throw new Exception("没有查询条件");
            }
            string sql = "select {0} from {1} where {2}";
            sql = string.Format(sql, ReflectionHelper.CreateFieldsName(model.GetType()), ReflectionHelper.CreateTableName(model.GetType()), model.WhereStr);
            if (model.Parameters.Count > 0)
            {
                IDataReader reader = MyDBHelper.QueryReader(sql, model.Parameters.ToArray());
                return FillDataGeneric<T>(reader);
            }
            else
            {
                IDataReader reader = MyDBHelper.QueryReader(sql);
                return FillDataGeneric<T>(reader);
            }
        }
        /// <summary>
        /// 根据查询条件获取一个实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static T Select<T>(this T model, Expression<Func<T, bool>> where) where T : Entity
        {
            model = model.Where(where);
            return model.Select();
        }
        /// <summary>
        /// 获取符合记录的数据有多少条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int SelectCount<T>(this T model, Expression<Func<T, bool>> where) where T : Entity
        {
            model = model.Where(where);
            return model.SelectCount();
        }
        /// <summary>
        /// 获取符合记录的数据有多少条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int SelectCount<T>(this T model) where T : Entity
        {
            if (string.IsNullOrEmpty(model.WhereStr))
            {
                throw new Exception("没有查询条件");
            }
            string sql = "select {0} from {1} where {2}";
            sql = string.Format(sql, " count(*) ", ReflectionHelper.CreateTableName(model.GetType()), model.WhereStr);
            if (model.Parameters.Count > 0)
            {
                DataTable dt = MyDBHelper.Query(sql, model.Parameters.ToArray());
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                DataTable dt = MyDBHelper.Query(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 根据实体类主键字段或是自定义条件更新该实体类数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Update<T>(this T model) where T : Entity
        {
            string msg = model.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            if (model.Parameters.Count == 0 || string.IsNullOrEmpty(model.WhereStr))//没有更新条件，自动根据主键创建更新条件
            {
                string sql = ReflectionHelper.CreateUpdate(model.GetType());
                List<IDataParameter> paras = new List<IDataParameter>();

                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        if (MyDBHelper.DbType == MyDbType.SqlServer)
                        {
                            #region 针对SQL的
                            SqlParameter para = new SqlParameter();
                            string fieldName = att.FieldName;
                            para.ParameterName = "@" + fieldName;
                            para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                            para.Value = property.GetValue(model, null);
                            paras.Add(para);
                            #endregion
                        }
                        else if (MyDBHelper.DbType == MyDbType.Oracle)
                        {
                            #region 针对oracle的
                            //OracleParameter para = new OracleParameter();
                            //string fieldName = att.FieldName;
                            //para.ParameterName = ":" + fieldName;
                            //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                            //para.Value = property.GetValue(model, null);
                            //paras.Add(para);
                            #endregion
                        }
                        else { }
                    }
                }
                if (paras.Count == 0)
                {
                    throw new Exception("该实体类没有主键字段，无法生成更新条件");
                }
                else
                {
                    return MyDBHelper.Execute(sql, paras.ToArray());
                }
            }
            else  //根据自定义的条件来进行更新
            {
                string sql = "{0} where {1}";
                sql = string.Format(sql, ReflectionHelper.CreateUpdateWithoutWhere(model.GetType()), model.WhereStr);
                return MyDBHelper.Execute(sql, model.Parameters.ToArray());
            }
        }
        /// <summary>
        /// 在数据库中保存该实体类数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Insert<T>(this T model) where T : Entity
        {
            string msg = model.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            string sql = ReflectionHelper.CreateInsert(model.GetType());
            List<IDataParameter> paras = new List<IDataParameter>();

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (atts.Length > 0)
                {
                    DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                    if (MyDBHelper.DbType == MyDbType.SqlServer)
                    {
                        #region for sql
                        SqlParameter para = new SqlParameter();
                        para.ParameterName = "@" + property.Name;
                        para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                        para.Value = property.GetValue(model, null);
                        paras.Add(para);
                        #endregion
                    }
                    else if (MyDBHelper.DbType == MyDbType.Oracle)
                    {
                        #region for oracle
                        //OracleParameter para = new OracleParameter();
                        //para.ParameterName = ":" + property.Name;
                        //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                        //para.Value = property.GetValue(model, null);
                        //paras.Add(para);
                        #endregion
                    }
                    else { }
                }
            }
            if (paras.Count == 0)
            {
                throw new Exception("没有实例化参数值");
            }
            return MyDBHelper.Execute(sql, paras.ToArray());
        }
        /// <summary>
        /// 在数据库中删除该实体类数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Delete<T>(this T model) where T : Entity
        {
            if (model.Parameters.Count == 0 && string.IsNullOrEmpty(model.WhereStr)) //没有删除条件，自动根据主键或是自增列创建更新条件
            {
                string sql = ReflectionHelper.CreateDelete(model.GetType());
                List<IDataParameter> paras = new List<IDataParameter>();

                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        if (att.IsPrimaryKey)
                        {
                            if (MyDBHelper.DbType == MyDbType.SqlServer)
                            {
                                #region for sql
                                SqlParameter para = new SqlParameter();
                                string fieldName = att.FieldName;
                                para.ParameterName = "@" + fieldName;
                                para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                                para.Value = property.GetValue(model, null);
                                paras.Add(para);
                                #endregion
                            }
                            else if (MyDBHelper.DbType == MyDbType.Oracle)
                            {
                                #region for oracle
                                //OracleParameter para = new OracleParameter();
                                //string fieldName = att.FieldName;
                                //para.ParameterName = ":" + fieldName;
                                //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                                //para.Value = property.GetValue(model, null);
                                //paras.Add(para);
                                #endregion
                            }
                            else { }
                        }
                    }
                }

                if (paras.Count == 0)
                {
                    throw new Exception("没有删除条件");
                }
                else
                {
                    return MyDBHelper.Execute(sql, paras.ToArray());
                }
            }
            else
            {
                string sql = "delete from {0} where {1}";
                sql = string.Format(sql, ReflectionHelper.CreateTableName(model.GetType()), model.WhereStr);
                return MyDBHelper.Execute(sql, model.Parameters.ToArray());
            }
        }
        /// <summary>
        /// 在数据库中删除该实体类数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="where">删除条件</param>
        /// <returns></returns>
        public static int Delete<T>(this T model, Expression<Func<T, bool>> where) where T : Entity
        {
            model = model.Where(where);
            return model.Delete();
        }
        #endregion

        #region 批量增删改
        /// <summary>
        /// 批量插入实体类(事务)(对于大量的数据不建议用该方式操作，例如list里面有上千条数据。。。)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int InsertList<T>(this List<T> modelList) where T : Entity
        {
            if (modelList.Count == 0)
            {
                return 0;
            }
            if (MyDBHelper.DbType == MyDbType.None)
            {
                throw new Exception("找不到对应的数据库类型");
            }
            string msg = "";
            foreach (Entity model in modelList)
            {
                msg += model.Validate();
            }
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            string sql = ReflectionHelper.CreateInsert(modelList[0].GetType());
            List<IDataParameter[]> paraList = new List<IDataParameter[]>();
            foreach (Entity model in modelList)
            {
                List<IDataParameter> paras = new List<IDataParameter>();
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        if (!att.IsIdentity)
                        {
                            if (MyDBHelper.DbType == MyDbType.SqlServer)
                            {
                                #region for sql
                                SqlParameter para = new SqlParameter();
                                para.ParameterName = "@" + property.Name;
                                para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                                para.Value = property.GetValue(model, null);
                                paras.Add(para);
                                #endregion
                            }
                            else if (MyDBHelper.DbType == MyDbType.Oracle)
                            {
                                #region for oracle
                                //OracleParameter para = new OracleParameter();
                                //para.ParameterName = ":" + property.Name;
                                //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                                //para.Value = property.GetValue(model, null);
                                //paras.Add(para);
                                #endregion
                            }
                            else { }
                        }
                    }
                }
                if (paras.Count > 0)
                {
                    paraList.Add(paras.ToArray());
                }
            }
            if (paraList.Count == 0)
            {
                throw new Exception("没有实例化参数值");
            }
            return MyDBHelper.ExecuteSql(sql, paraList);
        }
        /// <summary>
        /// 批量添加数据,适用大批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static bool InsertList2<T>(this List<T> modelList) where T : Entity
        {
            DataTable dt = modelList.ToDataTable();
            T RowInstance = Activator.CreateInstance<T>();
            string tableName = ReflectionHelper.CreateTableName(RowInstance.GetType());
            return MyDBHelper.AddDataBySqlBulkCopy(tableName, dt);
        }
        /// <summary>
        /// 批量根据实体类主键或自增列字段更新实体类(事务)（对于大量的数据不建议用该方式操作，例如list里面有上千条数据。。。）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int UpdateList<T>(this List<T> modelList) where T : Entity
        {
            if (modelList.Count == 0)
            {
                return 0;
            }
            if (MyDBHelper.DbType == MyDbType.None)
            {
                throw new Exception("找不到对应的数据库类型");
            }
            string msg = "";
            foreach (Entity model in modelList)
            {
                msg += model.Validate();
            }
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            string sql = ReflectionHelper.CreateUpdate(modelList[0].GetType());
            List<IDataParameter[]> paraList = new List<IDataParameter[]>();

            foreach (Entity model in modelList)
            {
                List<IDataParameter> paras = new List<IDataParameter>();
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        //if (att.IsPrimaryKey)
                        //{
                        if (MyDBHelper.DbType == MyDbType.SqlServer)
                        {
                            #region for sql
                            SqlParameter para = new SqlParameter();
                            para.ParameterName = "@" + property.Name;
                            para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                            para.Value = property.GetValue(model, null);
                            paras.Add(para);
                            #endregion
                        }
                        else if (MyDBHelper.DbType == MyDbType.SqlServer)
                        {
                            #region for oracle
                            //OracleParameter para = new OracleParameter();
                            //para.ParameterName = ":" + property.Name;
                            //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                            //para.Value = property.GetValue(model, null);
                            //paras.Add(para);
                            #endregion
                        }
                        else { }
                        //}
                    }
                }
                if (paras.Count > 0)
                {
                    paraList.Add(paras.ToArray());
                }
            }
            if (paraList.Count == 0)
            {
                throw new Exception("没有更新条件");
            }
            return MyDBHelper.ExecuteSql(sql, paraList);
        }
        /// <summary>
        /// 批量根据实体类主键字段删除实体类(事物)（对于大量的数据不建议用该方式操作，例如list里面有上千条数据。。。）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int DeleteList<T>(this List<T> modelList) where T : Entity
        {
            if (modelList.Count == 0)
            {
                return 0;
            }
            string sql = ReflectionHelper.CreateDelete(modelList[0].GetType());
            List<IDataParameter[]> paraList = new List<IDataParameter[]>();

            foreach (Entity model in modelList)
            {
                List<IDataParameter> paras = new List<IDataParameter>();
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object[] atts = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                    if (atts.Length > 0)
                    {
                        DataEntityFieldAttribute att = (DataEntityFieldAttribute)atts[0];
                        if (att.IsPrimaryKey)
                        {
                            if (MyDBHelper.DbType == MyDbType.SqlServer)
                            {
                                #region for sql
                                SqlParameter para = new SqlParameter();
                                para.ParameterName = "@" + property.Name;
                                para.SqlDbType = MyDBTypeConverter.GetSqlDbType(att.FieldType);
                                para.Value = property.GetValue(model, null);
                                paras.Add(para);
                                #endregion
                            }
                            else if (MyDBHelper.DbType == MyDbType.SqlServer)
                            {
                                #region for sql
                                //OracleParameter para = new OracleParameter();
                                //para.ParameterName = ":" + property.Name;
                                //para.DbType = MyDBTypeConverter.GetOracleDbType(att.FieldType);
                                //para.Value = property.GetValue(model, null);
                                //paras.Add(para);
                                #endregion
                            }
                            else { }
                        }
                    }
                }
                if (paras.Count > 0)
                {
                    paraList.Add(paras.ToArray());
                }
            }

            if (paraList.Count == 0)
            {
                throw new Exception("没有删除条件");
            }
            return MyDBHelper.ExecuteSql(sql, paraList);
        }


        #endregion

        #region 为实体类设置设置一个主键
        /// <summary>
        /// 为model类创建主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool GetPrimaryKeyValue<T>(this T model) where T : Entity
        {
            string tableName = ReflectionHelper.CreateTableName(model.GetType());
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                object[] att = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (att.Length > 0)
                {
                    DataEntityFieldAttribute attribute = (DataEntityFieldAttribute)att[0];
                    if (attribute.IsPrimaryKey)
                    {
                        string primaryValue = "";
                        if (MyDBHelper.DbType == MyDbType.SqlServer)
                        {
                            primaryValue = GetPrimaryKeyValue_SQL(tableName, attribute.FieldName);
                        }
                        else if (MyDBHelper.DbType == MyDbType.Oracle)
                        {
                            primaryValue = GetPrimaryKeyValue_ORACLE(tableName, attribute.FieldName);
                        }
                        else { }
                        if (string.IsNullOrEmpty(primaryValue))
                        {
                            return false;
                        }
                        else
                        {
                            property.SetValue(model, primaryValue, null);
                        }
                    }
                }
            }
            return true;
        }
        static string GetPrimaryKeyValue_SQL(string tableName, string fieldName)
        {
            string sql = @"declare @returnValue varchar(100)
EXEC  [dbo].[Sys_CreatePrimaryKeyValue] @tableName = N'{0}', @fieldName = N'{1}', @returnValue = @returnValue OUTPUT	
select @returnValue";
            sql = string.Format(sql, tableName, fieldName);

            DataTable ds = MyDBHelper.Query(sql);
            if (ds != null && ds.Rows.Count == 1)
            {
                object obj = ds.Rows[0][0];
                if (obj == null || obj.ToString().Trim() == "")
                {
                    throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
                }
                else
                {
                    return obj.ToString();
                }
            }
            else
            {
                throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
            }
        }
        static string GetPrimaryKeyValue_ORACLE(string tableName, string fieldName)
        {
            string sql = @"declare @returnValue varchar(100)
EXEC  [dbo].[Sys_CreatePrimaryKeyValue] @tableName = N'{0}', @fieldName = N'{1}', @returnValue = @returnValue OUTPUT	
select @returnValue";
            sql = string.Format(sql, tableName, fieldName);

            DataTable ds = MyDBHelper.Query(sql);
            if (ds != null && ds.Rows.Count == 1)
            {
                object obj = ds.Rows[0][0];
                if (obj == null || obj.ToString().Trim() == "")
                {
                    throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
                }
                else
                {
                    return obj.ToString();
                }
            }
            else
            {
                throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
            }
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 用IDataReader填充泛型List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> FillDataListGeneric<T>(System.Data.IDataReader reader)
        {

            List<T> DataList = new List<T>();
            if (reader == null)
            {
                return DataList;
            }
            DateTime t1 = DateTime.Now;
            PropertyInfo[] p = typeof(T).GetProperties();
            while (reader.Read())
            {
                T RowInstance = Activator.CreateInstance<T>();

                foreach (PropertyInfo P in p)
                {
                    if (P.IsDefined(typeof(DataEntityFieldAttribute), false))
                    {
                        int fieldIndex = reader.GetOrdinal(P.Name);
                        object obj = null;
                        if (fieldIndex >= 0)
                        {
                            obj = reader.GetValue(fieldIndex);
                        }
                        try
                        {
                            if (obj != DBNull.Value)
                            {
                                P.SetValue(RowInstance, obj, null);
                            }
                        }
                        catch
                        {
                        }
                    }

                }
                DataList.Add(RowInstance);
            }
            reader.Close();

            return DataList;
        }
        /// <summary>
        /// 用IDataReader填充一个数据实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T FillDataGeneric<T>(System.Data.IDataReader reader)
        {
            T RowInstance = Activator.CreateInstance<T>();
            while (reader.Read())
            {
                foreach (PropertyInfo P in typeof(T).GetProperties())
                {
                    if (P.IsDefined(typeof(DataEntityFieldAttribute), false))
                    {
                        int fieldIndex = reader.GetOrdinal(P.Name);
                        object obj = null;
                        if (fieldIndex >= 0)
                        {
                            obj = reader.GetValue(fieldIndex);
                        }
                        try
                        {
                            if (obj != DBNull.Value)
                            {
                                P.SetValue(RowInstance, obj, null);
                            }
                        }
                        catch { }
                    }
                }
            }
            reader.Close();
            return RowInstance;
        }
        /// <summary>
        /// 用datatable填充泛型List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<T> FillDataListGeneric<T>(System.Data.DataTable data)
        {
            List<T> DataList = new List<T>();
            if (data == null && data.Rows.Count == 0)
            {
                return DataList;
            }
            PropertyInfo[] p = typeof(T).GetProperties();
            foreach (DataRow dr in data.Rows)
            {
                T RowInstance = Activator.CreateInstance<T>();
                foreach (PropertyInfo pro in p)
                {
                    if (pro.IsDefined(typeof(DataEntityFieldAttribute), false))
                    {
                        try
                        {
                            if (pro.IsDefined(typeof(DataEntityFieldAttribute), false))
                            {
                                var obj = dr[pro.Name];
                                if (obj != DBNull.Value)
                                {
                                    pro.SetValue(RowInstance, obj, null);
                                }
                            }
                        }
                        catch { }
                    }
                }
                DataList.Add(RowInstance);
            }
            return DataList;
        }
        /// <summary>
        /// 用datatable填充一个数据实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T FillDataGeneric<T>(System.Data.DataTable data)
        {
            T RowInstance = Activator.CreateInstance<T>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (PropertyInfo P in typeof(T).GetProperties())
                {
                    if (P.IsDefined(typeof(DataEntityFieldAttribute), false))
                    {
                        try
                        {
                            object obj = data.Rows[0][P.Name];
                            if (obj != DBNull.Value)
                            {
                                P.SetValue(RowInstance, obj, null);
                            }
                        }
                        catch { }
                    }
                }
            }
            return RowInstance;
        }
        #endregion
    }

    public static class BaseServiceExpand
    {
        #region 为实体类集合设置主键
        /// <summary>
        /// 为entity类创建主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelList"></param>
        public static void GetMultiPrimaryKeyValue<T>(this List<T> modelList) where T : Entity
        {

            List<string> primaryList = new List<string>();
            string tableName = ReflectionHelper.CreateTableName(modelList[0].GetType());
            foreach (PropertyInfo property in modelList[0].GetType().GetProperties())
            {
                object[] att = property.GetCustomAttributes(typeof(DataEntityFieldAttribute), false);
                if (att.Length > 0)
                {
                    DataEntityFieldAttribute attribute = (DataEntityFieldAttribute)att[0];
                    if (attribute.IsPrimaryKey)
                    {
                        if (MyDBHelper.DbType == MyDbType.SqlServer)
                        {
                            primaryList = GetMultiPrimaryKeyValue_SQL(tableName, attribute.FieldName, modelList.Count.ToString());
                        }
                        else if (MyDBHelper.DbType == MyDbType.Oracle)
                        {
                            primaryList = GetMultiPrimaryKeyValue_ORACLE(tableName, attribute.FieldName, modelList.Count.ToString());
                        }
                        else { }
                        if (primaryList.Count > 0 && primaryList.Count == modelList.Count)
                        {
                            for (int i = 0; i < primaryList.Count; i++)
                            {
                                property.SetValue(modelList[i], primaryList[i], null);
                            }

                        }
                    }
                }
            }
        }

        static List<string> GetMultiPrimaryKeyValue_SQL(string tableName, string fieldName, string number)
        {
            string sql = @"EXEC  [dbo].[Sys_CreateMorePrimaryKeyValues] @tableName = N'{0}', @fieldName = N'{1}', @number = N'{2}'";
            sql = string.Format(sql, tableName, fieldName, number);

            DataTable ds = MyDBHelper.Query(sql);
            List<string> strKeys = new List<string>();

            if (ds != null && ds.Rows.Count == int.Parse(number))
            {
                object obj = ds.Rows[0][0];
                if (obj == null || obj.ToString().Trim() == "")
                {
                    throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
                }
                else
                {
                    foreach (DataRow dr in ds.Rows)
                    {
                        strKeys.Add(dr[0].ToString());
                    }
                    return strKeys;
                }
            }
            else
            {
                throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
            }
        }
        static List<string> GetMultiPrimaryKeyValue_ORACLE(string tableName, string fieldName, string number)
        {
            string sql = @"EXEC  [dbo].[Sys_CreateMorePrimaryKeyValues] @tableName = N'{0}', @fieldName = N'{1}', @number = N'{2}'";
            sql = string.Format(sql, tableName, fieldName, number);

            DataTable ds = MyDBHelper.Query(sql);
            List<string> strKeys = new List<string>();

            if (ds != null && ds.Rows.Count == int.Parse(number))
            {
                object obj = ds.Rows[0][0];
                if (obj == null || obj.ToString().Trim() == "")
                {
                    throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
                }
                else
                {
                    foreach (DataRow dr in ds.Rows)
                    {
                        strKeys.Add(dr[0].ToString());
                    }
                    return strKeys;
                }
            }
            else
            {
                throw new Exception(string.Format("请现在系统表System_Primerykey中配置{0}表的主键生成规则", tableName));
            }
        }
        #endregion
    }

    /// <summary>
    /// 集合类按某个数据数据过滤重复行的扩展方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
    {
        private Func<T, V> keySelector;
        private IEqualityComparer<V> comparer;

        public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public CommonEqualityComparer(Func<T, V> keySelector)
            : this(keySelector, EqualityComparer<V>.Default)
        { }

        public bool Equals(T x, T y)
        {
            return comparer.Equals(keySelector(x), keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return comparer.GetHashCode(keySelector(obj));
        }
    }
    /// <summary>
    /// 提供oracle和sql数据类型与c#的数据类型之间的对应
    /// </summary>
    internal static class MyDBTypeConverter
    {
        #region dbtype辅助转换类
        public static SqlDbType GetSqlDbType(string type)
        {
            SqlDbType _type = SqlDbType.VarChar;
            switch (type.ToLower())
            {
                case "bigint":
                    _type = SqlDbType.BigInt;
                    break;
                case "binary":
                    _type = SqlDbType.Binary;
                    break;
                case "bit":
                    _type = SqlDbType.Bit;
                    break;
                case "char":
                    _type = SqlDbType.Char;
                    break;
                case "date":
                    _type = SqlDbType.Date;
                    break;
                case "datetime":
                    _type = SqlDbType.DateTime;
                    break;
                case "datetime2":
                    _type = SqlDbType.DateTime2;
                    break;
                case "datetimeoffset":
                    _type = SqlDbType.DateTimeOffset;
                    break;
                case "decimal":
                    _type = SqlDbType.Decimal;
                    break;
                case "float":
                    _type = SqlDbType.Float;
                    break;
                case "image":
                    _type = SqlDbType.Image;
                    break;
                case "int":
                    _type = SqlDbType.Int;
                    break;
                case "money":
                    _type = SqlDbType.Money;
                    break;
                case "nchar":
                    _type = SqlDbType.NChar;
                    break;
                case "ntext":
                    _type = SqlDbType.NText;
                    break;
                case "numeric":
                    _type = SqlDbType.Decimal;
                    break;
                case "nvarchar":
                    _type = SqlDbType.NVarChar;
                    break;
                case "real":
                    _type = SqlDbType.Real;
                    break;
                case "smalldatetime":
                    _type = SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    _type = SqlDbType.SmallInt;
                    break;
                case "smallmoney":
                    _type = SqlDbType.SmallMoney;
                    break;
                case "sql_variant":
                    _type = SqlDbType.Variant;
                    break;
                case "text":
                    _type = SqlDbType.Text;
                    break;
                case "time":
                    _type = SqlDbType.Time;
                    break;
                case "timestamp":
                    _type = SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    _type = SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    _type = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    _type = SqlDbType.VarBinary;
                    break;
            }
            return _type;
        }
        public static DbType GetOracleDbType(string type)
        {
            DbType _type = DbType.String;
            switch (type.ToLower())
            {
                case "blob":
                    _type = DbType.Binary;
                    break;
                case "bfile":
                    _type = DbType.Binary;
                    break;
                case "char":
                    _type = DbType.String;
                    break;
                case "varchar2":
                    _type = DbType.String;
                    break;
                case "clob":
                    _type = DbType.String;
                    break;
                case "date":
                    _type = DbType.DateTime;
                    break;
                case "number":
                    _type = DbType.Decimal;
                    break;
                case "float":
                    _type = DbType.Decimal;
                    break;
                case "long":
                    _type = DbType.String;
                    break;
                case "nchar":
                    _type = DbType.String;
                    break;
                case "nvarchar2":
                    _type = DbType.String;
                    break;
                case "nclob":
                    _type = DbType.String;
                    break;
                case "raw":
                    _type = DbType.Binary;
                    break;
                case "rowid":
                    _type = DbType.String;
                    break;
                case "timestamp(6)":
                    _type = DbType.DateTime;
                    break;
                case "urowid":
                    _type = DbType.String;
                    break;
            }
            return _type;
        }
        #endregion
    }
    /// <summary>
    /// 数据库事务操作
    /// </summary>
    public class DBTransaction
    {
        private Action action;
        /// <summary>
        /// 将多个数据库操作放到一个事物中
        /// <para>注意，只需要把最终数据修改部分的操作放入事务中，需要回滚事务的时候，可以手动抛出异常，即可回滚事务</para>
        /// <para>单个的数据操作，不需要放在这个事务中执行</para>
        /// <para>尽量不要把业务逻辑判断操作放到事务中</para>
        /// </summary>
        /// <param name="action"></param>
        public void TransferScope(Action action)
        {
            this.action = action;
        }
        /// <summary>
        /// 提交事务，返回操作结果，如果失败，就返回失败的原因，否则返回空
        /// </summary>
        /// <returns></returns>
        public string SubmitTransfer()
        {
            if (this.action == null)
            {
                return "没有需要提交的事务操作";
            }
            string result = "";

            TransactionOptions transactionOption = new TransactionOptions();
            #region System.Transactions.IsolationLevel的用法
            //Chaos 无法改写隔离级别更高的事务中的挂起的更改。   
            //ReadCommitted 在正在读取数据时保持共享锁，以避免脏读，但是在事务结束之前可以更改数据，从而导致不可重复的读取或幻像数据。   
            //ReadUncommitted 可以进行脏读，意思是说，不发布共享锁，也不接受 独占锁。   
            //RepeatableRead 在查询中使用的所有数据上放置锁，以防止其他用户更新这些数据。防止不可重复的读取，但是仍可以有幻像行。   
            //Serializable 在 DataSet 上放置范围锁，以防止在事务完成之前由其他用户更新行或向数据集中插入行。   
            //Snapshot 通过在一个应用程序正在修改数据时存储另一个应用程序可以读取的相同数据版本来减少阻止。表示您无法从一个事务中看到在其他事务中进行的更改，即便重新查询也是如此。   
            //Unspecified 正在使用与指定隔离级别不同的隔离级别，但是无法确定该级别。  
            //当使用 OdbcTransaction 时，如果不设置 IsolationLevel 或者将 IsolationLevel 设置为 Unspecied，事务将根据基础 ODBC 驱动程序的默认隔离级别来执行。
            #endregion
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            transactionOption.Timeout = new TimeSpan(0, 10, 0);
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                try
                {
                    this.action();
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                finally
                {
                    transactionScope.Dispose();
                }
            }
            this.action = null;
            return result;
        }
    }
}
