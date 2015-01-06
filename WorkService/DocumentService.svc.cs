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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DocumentService : IDocumentService
    {
        private int nCount = 0;

        public void SetResult(int value)
        {
            nCount = value;
        }

        public string GetResult()
        {
            return (nCount).ToString();
        }

        public string GetData(string value)
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
            MyDBHelper.InitConnectionString();
            codeUsers user = new codeUsers();
            bool ok = user.SelectCount(u => u.mail == mail && u.mailpwd == pwd) > 0;
            return new Models.ServerMessage()
            {
                Success = ok,
                Message = ok ? "登陆成功" : "登陆失败"
            };
        }
        
        public Models.ServerMessage AddDiary(int userId, string title, string content)
        {
            var failResult = new Models.ServerMessage()
            {
                Success = false,
                Message = "保存失败"
            };

            domainItems item = new domainItems();
            item.date = DateTime.Now.Date;
            item.userId = userId;
            item.name = title;
            item.valid = true;
            if (item.Insert() == 1)
            {
                domainText text = new domainText();
                text.itemId = item.id;
                text.text = content;
                if (text.Insert() == 1)
                {
                    return new Models.ServerMessage()
                    {
                        Success = true,
                        Message = "保存成功"
                    };
                }
                else failResult.Message = "domainText 保存失败";
            }
            else failResult.Message = "domainItems 保存失败";

            return failResult;
        }


        public void GetSessionId()
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            callback.CallbackAction(OperationContext.Current.SessionId);
        }
    }
}
