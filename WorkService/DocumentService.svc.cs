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
using _3rd;

namespace WorkService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DocumentService : IDocumentService
    {


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
            codeUsers user = new codeUsers().Select(u => u.number == number);
            callback.ReturnUserInfo(user);
        }


        public void LoadDiary(int id)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.id == id);
            var ds = diarys.Select();
            callback.ReturnUserDiary(ds);
        }

        public void LoadDiarys(int userid, DateTime date)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.userId == userid && w.date == date);
            var ds = diarys.SelectList();
            callback.ReturnDiaryItems(ds);
        }

        public void InsertDiaryItems(IEnumerable<domainDiary> diaryItems)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();

            int i = 0;
            foreach (var item in diaryItems)
            {
                item.Insert();
                i++;
                int percent = (int)(((double)i / (double)((long)diaryItems.Count())) * 100);
                callback.ReturnDiaryItemInsertPercent(percent);
            }
        }


        #region Send Diary
        public void SendDiary(string number, DateTime date)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();

            vDiarys diary = new vDiarys();
            diary.Where(w => w.number == number && w.date == date.Date);
            var ds = diary.SelectList();

            string templateFile = new codeSettings().Where(c => c.name == "templatefile").Select().value;
            string newfilename = __getNewFilename(number, date);
            using (var eh = new DiaryExcelHelper(templateFile))
            {
                foreach (var d in ds)
                {
                    eh.InsertItem(d);
                }

                eh.SaveAs(newfilename);
            }

            bool sendOk = __sendMail(number, newfilename);

            callback.ReturnSendDiary(sendOk, sendOk ? "日志发送成功" : "日志发送失败");
        }

        private bool __sendMail(string number, string newfilename)
        {
            codeUsers user = new codeUsers().Where(u => u.number == number).Select();

            Email email = new Email();
            email.host = "smtp.qq.com";
            email.port = 587;
            email.mailFrom = user.mail;
            email.mailPwd = _3rd.Security.Decode(user.mailpwd);
            email.mailSubject = System.IO.Path.GetFileNameWithoutExtension(newfilename) + " " + user.name;
            email.mailToArray = user.mailto.Split(';');
            email.attachmentsPath = new string[] { newfilename };
            return email.Send();
        }

        private string __getNewFilename(string number, DateTime date)
        {
            //生成新的文件名
            string sql = string.Format(@"select  d.code + '_' + u.number  + '_' from codeUsers u inner join codeDeparts d on u.departId = d.id and u.number = '{0}'", number);

            string prefix = (string)MyDBHelper.QueryScalar(sql);

            string d = date.ToString("yyyyMMdd");

            return prefix + d + ".xlsx";
        } 
        #endregion


        public void InsertUser(codeUsers user)
        {
            int r = user.Insert();
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            callback.ReturnRowAffected(r);
        }

        public void UpdateUser(codeUsers user)
        {
            user.Update();
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            callback.ReturnUserInfo(new codeUsers().Where(v => v.number == user.number).Select());
        }
    }

}
