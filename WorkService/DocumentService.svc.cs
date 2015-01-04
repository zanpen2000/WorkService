using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DBHelper;
using Model;

namespace WorkService
{
    public class DocumentService : IDocumentService
    {


        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public Models.ServerMessage RegisNewUser(codeUsers user)
        {
            if (string.IsNullOrEmpty(user.mail))
            {
                return new Models.ServerMessage()
                {
                    Success = false,
                    Message = "mail不能为空"
                };
            }
            else if (user.SelectCount(u => u.mail == user.mail) < 1)
            {
                int count = user.Insert();
                Models.ServerMessage msg = new Models.ServerMessage()
                {
                    Success = count > 0,
                    Message = string.Format("插入了{0}条数据", count)
                };
                return msg;
            }
            else
                return new Models.ServerMessage()
                {
                    Success = false,
                    Message = string.Format("用户 {0} 已存在", user.mail)
                };
        }

        public Models.ServerMessage Login(string mail, string pwd)
        {
            codeUsers user = new codeUsers();
            bool ok = user.SelectCount(u => u.mail == mail && u.mailpwd == pwd) > 0;
            return new Models.ServerMessage()
            {
                Success = ok,
                Message = ok ? "登陆成功" : "登陆失败"
            };
        }
    }
}
