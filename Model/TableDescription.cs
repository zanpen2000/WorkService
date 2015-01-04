using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 用于描述实体类字段和数据库字段的对应关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataEntityFieldAttribute : Attribute
    {
        private string _fieldName = "";
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
        private string _fieldRemark = "";
        /// <summary>
        /// 描述
        /// </summary>
        public string FieldRemark
        {
            get { return _fieldRemark; }
            set { _fieldRemark = value; }
        }
        private string _fieldType = "";
        /// <summary>
        /// 数据类型
        /// </summary>
        public string FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }
        private bool _isPrimaryKey = false;
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }
        private bool _allowNull = true;
        /// <summary>
        /// 允许空
        /// </summary>
        public bool AllowNull
        {
            get { return _allowNull; }
            set { _allowNull = value; }
        }
        private string _defaultValue = "";
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
        private int _length = 10;
        /// <summary>
        /// 字段长度
        /// </summary>
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private bool isIdentity = false;
        /// <summary>
        /// 标识列
        /// </summary>
        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        public DataEntityFieldAttribute() { }
        public DataEntityFieldAttribute(string fieldName, string fieldType, int length, bool isPrimaryKey, bool isIdentity, bool allowNull, string defaultValue)
        {
            this._length = length;
            this._fieldName = fieldName;
            this._fieldType = fieldType;
            this._isPrimaryKey = isPrimaryKey;
            this.isIdentity = isIdentity;
            this._allowNull = allowNull;
            this._defaultValue = defaultValue;
        }
        public DataEntityFieldAttribute(string fieldName, string fieldType, int length, bool isPrimaryKey, bool isIdentity, bool allowNull)
        {
            this._length = length;
            this._fieldName = fieldName;
            this._fieldType = fieldType;
            this._isPrimaryKey = isPrimaryKey;
            this.isIdentity = isIdentity;
            this._allowNull = allowNull;
            this._defaultValue = string.Empty;
        }
        public DataEntityFieldAttribute(string fieldName, string fieldType, string fieldRemark, int length, bool isPrimaryKey, bool isIdentity, bool allowNull)
        {
            this._length = length;
            this._fieldName = fieldName;
            this._fieldType = fieldType;
            this._isPrimaryKey = isPrimaryKey;
            this.isIdentity = isIdentity;
            this._allowNull = allowNull;
            this._defaultValue = string.Empty;
            this._fieldRemark = fieldRemark;
        }
        public DataEntityFieldAttribute(string fieldName, string fieldType, int length, bool isPrimaryKey)
        {
            this._fieldName = fieldName;
            this._length = length;
            this._fieldType = fieldType;
            this._isPrimaryKey = isPrimaryKey;
            this._allowNull = true;
            this._defaultValue = string.Empty;
        }
        public DataEntityFieldAttribute(string fieldName, string fieldType, int length)
        {
            this._length = length;
            this._fieldName = fieldName;
            this._fieldType = fieldType;
            this._isPrimaryKey = false;
            this._allowNull = true;
            this._defaultValue = string.Empty;
        }
    }
    /// <summary>
    /// 用于描述实体类和数据库的表对应关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataEntityAttribute : Attribute
    {
        private string _tableName = "";

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public DataEntityAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
