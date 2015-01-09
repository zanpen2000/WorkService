using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DBHelper;

using ServiceContract;
using ServiceContract.Models;
using DBModel;

namespace WorkService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DocumentService : IDocumentService
    {
        public ServerMessage RegisNewUser(codeUsers user)
        {
            if (string.IsNullOrEmpty(user.mail))
            {
                return new ServerMessage()
                {
                    Success = false,
                    Message = "mail不能为空"
                };
            }
            else if (user.SelectCount(u => u.mail == user.mail) < 1)
            {
                int count = user.Insert();
                ServerMessage msg = new ServerMessage()
                {
                    Success = count > 0,
                    Message = string.Format("插入了{0}条数据", count)
                };
                return msg;
            }
            else
                return new ServerMessage()
                {
                    Success = false,
                    Message = string.Format("用户 {0} 已存在", user.mail)
                };
        }

        public ServerMessage Login(string mail, string pwd)
        {
            codeUsers user = new codeUsers();
            bool ok = user.SelectCount(u => u.mail == mail && u.mailpwd == pwd) > 0;
            return new ServerMessage()
            {
                Success = ok,
                Message = ok ? "登陆成功" : "登陆失败"
            };
        }

        public ServerMessage AddDiary(int userId, string title, string content)
        {
            var failResult = new ServerMessage()
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
                    return new ServerMessage()
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

        public void GetUserDiarys(string userNum, string currentpage)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            viewUserDiarys diarys = new viewUserDiarys();
            diarys.Where(w => w.number == userNum);
            PagerList<viewUserDiarys> ds = diarys.SelectPageList(currentpage, "20", "lastsent", "0", "number");
            callback.ReturnUserDiarys(ds);
        }

        public void GetUserInfo(string number)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            viewUserInfo user = new viewUserInfo().Select(u => u.number == number);
            callback.ReturnUserInfo(user);
        }


        public void LoadDiary(int id)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w=>w.id == id);
            var ds =diarys.Select();
            callback.ReturnUserDiary(ds);
        }
    }

}
