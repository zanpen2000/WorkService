

 
using System;
using System.Collections.Generic;
using System.Text;


namespace Model
{     
				
				/// <summary>
				/// 
				/// </summary>
				[DataEntityAttribute("codeDeparts")]
				public class codeDeparts : Entity
				{
					
			
					private System.Int32 _id = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "id", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = true, IsIdentity = true, AllowNull = false)]
					public System.Int32 id{ set { _id = value; changed("id");} get { return _id; } }

				
					private System.String _name = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "name", FieldType = "nvarchar", FieldRemark = "", Length = 50, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.String name{ set { _name = value; changed("name");} get { return _name; } }

				
					private System.Boolean _valid = true;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "valid", FieldType = "bit", FieldRemark = "", Length = 1, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.Boolean valid{ set { _valid = value; changed("valid");} get { return _valid; } }

				}


				
				/// <summary>
				/// 
				/// </summary>
				[DataEntityAttribute("codeSettings")]
				public class codeSettings : Entity
				{
					
			
					private System.Int32 _id = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "id", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = true, IsIdentity = true, AllowNull = false)]
					public System.Int32 id{ set { _id = value; changed("id");} get { return _id; } }

				
					private System.String _name = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "name", FieldType = "varchar", FieldRemark = "", Length = 100, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.String name{ set { _name = value; changed("name");} get { return _name; } }

				
					private System.String _value = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "value", FieldType = "nvarchar", FieldRemark = "", Length = -1, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String value{ set { _value = value; changed("value");} get { return _value; } }

				
					private System.String _description = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "description", FieldType = "nvarchar", FieldRemark = "", Length = -1, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String description{ set { _description = value; changed("description");} get { return _description; } }

				}


				
				/// <summary>
				/// 
				/// </summary>
				[DataEntityAttribute("codeUsers")]
				public class codeUsers : Entity
				{
					
			
					private System.Int32 _id = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "id", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = true, IsIdentity = true, AllowNull = false)]
					public System.Int32 id{ set { _id = value; changed("id");} get { return _id; } }

				
					private System.String _number = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "number", FieldType = "varchar", FieldRemark = "", Length = 30, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.String number{ set { _number = value; changed("number");} get { return _number; } }

				
					private System.String _name = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "name", FieldType = "nvarchar", FieldRemark = "", Length = 30, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.String name{ set { _name = value; changed("name");} get { return _name; } }

				
					private System.DateTime? _lastSent = null;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "lastSent", FieldType = "datetime", FieldRemark = "", Length = 23, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.DateTime? lastSent{ set { _lastSent = value; changed("lastSent");} get { return _lastSent; } }

				
					private System.Int32 _departId = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "departId", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.Int32 departId{ set { _departId = value; changed("departId");} get { return _departId; } }

				
					private System.String _mail = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "mail", FieldType = "varchar", FieldRemark = "", Length = 100, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String mail{ set { _mail = value; changed("mail");} get { return _mail; } }

				
					private System.String _mailpwd = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "mailpwd", FieldType = "varchar", FieldRemark = "", Length = 255, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String mailpwd{ set { _mailpwd = value; changed("mailpwd");} get { return _mailpwd; } }

				
					private System.String _mailto = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "mailto", FieldType = "varchar", FieldRemark = "", Length = -1, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String mailto{ set { _mailto = value; changed("mailto");} get { return _mailto; } }

				}


				
				/// <summary>
				/// 
				/// </summary>
				[DataEntityAttribute("domainItems")]
				public class domainItems : Entity
				{
					
			
					private System.Int32 _id = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "id", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = true, IsIdentity = true, AllowNull = false)]
					public System.Int32 id{ set { _id = value; changed("id");} get { return _id; } }

				
					private System.Int32 _userId = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "userId", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.Int32 userId{ set { _userId = value; changed("userId");} get { return _userId; } }

				
					private System.DateTime _date = DateTime.Now;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "date", FieldType = "date", FieldRemark = "", Length = 10, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.DateTime date{ set { _date = value; changed("date");} get { return _date; } }

				
					private System.String _pos = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "pos", FieldType = "varchar", FieldRemark = "", Length = 3, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String pos{ set { _pos = value; changed("pos");} get { return _pos; } }

				
					private System.String _name = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "name", FieldType = "nvarchar", FieldRemark = "", Length = -1, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.String name{ set { _name = value; changed("name");} get { return _name; } }

				
					private System.Boolean _valid = true;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "valid", FieldType = "bit", FieldRemark = "", Length = 1, IsPrimaryKey = false, IsIdentity = false, AllowNull = true)]
					public System.Boolean valid{ set { _valid = value; changed("valid");} get { return _valid; } }

				}


				
				/// <summary>
				/// 
				/// </summary>
				[DataEntityAttribute("domainText")]
				public class domainText : Entity
				{
					
			
					private System.Int32 _id = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "id", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = true, IsIdentity = true, AllowNull = false)]
					public System.Int32 id{ set { _id = value; changed("id");} get { return _id; } }

				
					private System.Int32 _itemId = 0;
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "itemId", FieldType = "int", FieldRemark = "", Length = 10, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.Int32 itemId{ set { _itemId = value; changed("itemId");} get { return _itemId; } }

				
					private System.String _text = "";
					/// <summary>
					/// 
					/// </summary>
					[DataEntityFieldAttribute(FieldName = "text", FieldType = "nvarchar", FieldRemark = "", Length = -1, IsPrimaryKey = false, IsIdentity = false, AllowNull = false)]
					public System.String text{ set { _text = value; changed("text");} get { return _text; } }

				}


	
}