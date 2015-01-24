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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DocumentService : IDocumentService
    {
        public void GetUserDiarys(string userNum, string currentpage)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            viewUserDiarys diarys = new viewUserDiarys();
            diarys.Where(w => w.number == userNum);
            PagerList<viewUserDiarys> ds = diarys.SelectPageList(currentpage, "20", "lastsent", "0", "number");
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnUserDiarys(ds);
        }

        public void GetUserInfo(string number)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            codeUsers user = new codeUsers().Select(u => u.number == number);
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnUserInfo(user);
        }


        public void LoadDiary(int id)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.id == id);
            var ds = diarys.Select();
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnUserDiary(ds);
        }

        public void LoadDiarys(int userid, DateTime date)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.userId == userid && w.date == date);
            var ds = diarys.SelectList();
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
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
                if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                    callback.ReturnDiaryItemInsertPercent(percent);
            }
        }


        #region Send Diary
        public void SendDiary(string number, DateTime date)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();

            string newfilename = SaveExcelFile(number, date);



            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnSaveExcelFile(System.IO.Path.GetFileName(newfilename));

            bool sendOk = __sendMail(number, newfilename);
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnSendDiary(sendOk, sendOk ? "日志发送成功" : "日志发送失败");
        }

        string SaveExcelFile(string number, DateTime date)
        {
            vDiarys diary = new vDiarys();
            diary.Where(w => w.number == number && w.date == date.Date);
            var ds = diary.SelectList();

            codeSettings settings = new codeSettings();

            string templateFile = settings.Where(c => c.name == "templatefile").Select().value;
            templateFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, templateFile);
            string newfilename = __getNewFilename(number, date);

            if (!System.IO.Directory.Exists(newfilename))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newfilename));

            if (System.IO.File.Exists(newfilename)) System.IO.File.Delete(newfilename);

            using (var eh = new DiaryExcelHelper(templateFile))
            {
                eh.RowToInsert = int.Parse(settings.Where(s => s.name == "RowToInsert").Select().value);
                eh.NumberPosition = settings.Where(s => s.name == "NumberPosition").Select().value;
                eh.NamePosition = settings.Where(s => s.name == "NamePosition").Select().value;
                eh.DepartPosition = settings.Where(s => s.name == "DepartPosition").Select().value;
                eh.DatePosition = settings.Where(s => s.name == "DatePosition").Select().value;

                foreach (var d in ds)
                {
                    eh.InsertItem(d);
                }

                eh.SaveAs(newfilename);
            }

            var usr = new codeUsers().Where(u => u.number == number).Select();

            domainFiles dfile = new domainFiles();
            dfile.date = date;
            dfile.filepath = newfilename;
            dfile.userId = usr.id;
            dfile.sent = false;
            dfile.Insert();

            domainDiary dd = new domainDiary();
            var dss = dd.Where(d => d.userId == usr.id && d.date == date).SelectList();

            foreach (var item in dss)
            {
                item.fileId = new domainFiles().Where(files => files.date == date && files.filepath == newfilename && files.userId == usr.id).Select().id;
                item.Update();
            }
         
            return newfilename;
        }

        private bool __sendMail(string number, string newfilename)
        {
            codeUsers user = new codeUsers().Where(u => u.number == number).Select();

            Email email = new Email();
            email.host = "smtp.qq.com";
            email.port = 587;
            email.mailFrom = user.mail;
            //email.mailPwd = user.mailpwd;
            email.mailPwd = _3rd.Security.Decode(user.mailpwd);
            email.mailSubject = System.IO.Path.GetFileNameWithoutExtension(newfilename) + " " + user.name;
            email.mailToArray = user.mailto.Split(';');
            email.attachmentsPath = new string[] { newfilename };

            bool result = email.Send();

            if (result)
            {
                domainFiles dfile = new domainFiles();
                var df = dfile.Where(f => f.filepath == newfilename).Select();
                df.sent = true;
                df.Update();


            }
            return result;
        }

        string __getNewFilename(string number, DateTime date)
        {
            //生成新的文件名
            string sql = string.Format(@"select  d.code + '_' + u.number  + '_' from codeUsers u inner join codeDeparts d on u.departId = d.id and u.number = '{0}'", number);

            string prefix = (string)MyDBHelper.QueryScalar(sql);

            string d = date.ToString("yyyyMMdd");

            string filename = prefix + d + ".xlsx";

            codeSettings settings = new codeSettings();
            settings.Where(s => s.name == "diaryPath");
            settings = settings.Select();
            string path = string.Format(settings.value, number);

            string filepath = System.IO.Path.Combine(path, filename);
            filepath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filepath);
            return filepath;
        }
        #endregion

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user"></param>
        public void EditUser(codeUsers user)
        {
            int rowAffected = 0;

            if (new codeUsers().SelectCount(u => u.number == user.number) > 0)
            {
                rowAffected = user.Update();
            }
            else
            {
                rowAffected = user.Insert();
            }
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            if (OperationContext.Current.Channel.State == CommunicationState.Opened)
                callback.ReturnRowAffected(rowAffected);
        }

    }

}
