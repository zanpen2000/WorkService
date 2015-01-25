
using AppLayer;
using DBModel;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DesktopClient.Model
{
    public class DataService : IDataService
    {
        public event EventHandler<ServiceContract.UserInfoEventArgs> OnGetUserInfo = delegate { };
        public event EventHandler<ServiceContract.ViewDiarysEventArgs> OnGetUserDiarys = delegate { };
        public event EventHandler<ServiceContract.DiarysEventArgs> OnLoadDiarys = delegate { };
        public event EventHandler<ServiceContract.DiaryEventArgs> OnLoadDiary;
        public event EventHandler<ServiceContract.DiaryItemsInsertEventArgs> OnDiaryItemsInsert = delegate { };
        public event EventHandler<ServiceContract.RowAffectedEventArgs> OnSavedToDatabase = delegate { };
        public event EventHandler<ServerExcelFilenameEventArg> OnSavedExcelFile = delegate { };
        public event EventHandler<ServerSendDiaryEventArg> OnServerSendDiary;
        public event EventHandler<ItemNameExistsEventArg> OnItemNameExists;

        public void GetUserInfo()
        {
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.GetUserInfo(userNumber);
            });
        }

        public void ReturnUserInfo(codeUsers user)
        {
            OnGetUserInfo(this, new ServiceContract.UserInfoEventArgs(user));
        }

        public void GetDiarys(string page)
        {
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.GetUserDiarys(userNumber, page);
            });
        }
        public void ReturnUserDiarys(System.Collections.Generic.IEnumerable<viewUserDiarys> diarys)
        {
            OnGetUserDiarys(this, new ServiceContract.ViewDiarysEventArgs(diarys));
        }

        public void LoadDiary(int id)
        {
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.LoadDiary(id);
            });
        }

        public void ReturnUserDiary(domainDiary diary)
        {
            OnLoadDiary(this, new ServiceContract.DiaryEventArgs(diary));
        }

        public void ReturnDiaryItems(System.Collections.Generic.IEnumerable<domainDiary> items)
        {
            OnLoadDiarys(this, new ServiceContract.DiarysEventArgs(items));
        }

        public void LoadDiaryItems(int userId, DateTime date)
        {
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.LoadDiarys(userId, date);
            });
        }

        public void InsertDiaryItems(System.Collections.Generic.IEnumerable<domainDiary> diaryItems)
        {
            List<domainDiary> diarys = new List<domainDiary>();
            foreach (var item in diaryItems)
            {
                if (item.fileId <= 0) diarys.Add(item);
            }

            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.InsertDiaryItems(diarys);
            });
        }

        public void ReturnDiaryItemInsertPercent(int percent)
        {
            OnDiaryItemsInsert(this, new ServiceContract.DiaryItemsInsertEventArgs(percent));
        }


        public void ReturnSendDiary(bool successed, string msg)
        {
            OnServerSendDiary(this, new ServerSendDiaryEventArg(successed, msg));
        }


        public void EditUser(codeUsers user)
        {
            AppSettings.Set("Number", user.number);

            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.EditUser(user);
            });
        }

        public void ReturnRowAffected(int r)
        {
            OnSavedToDatabase(this, new ServiceContract.RowAffectedEventArgs(r));
        }

        public void SendDiary(string number, DateTime date)
        {
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.SendDiary(number, date);
            });
        }

        public void ReturnSaveExcelFile(string newfilename)
        {
            OnSavedExcelFile(this, new ServerExcelFilenameEventArg(newfilename));
        }

        public void CheckItemNameExists(string itemname)
        {
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.CheckItemNameExists(itemname);
            });
        }

        public void ReturnItemNameExists(bool exists)
        {
            OnItemNameExists(this, new ItemNameExistsEventArg(exists));
        }
    }
}