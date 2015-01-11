
using AppLayer;
using DBModel;
using System;
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
        

        public void GetUserInfo()
        {
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.GetUserInfo(userNumber);
            });
        }

        public void ReturnUserInfo(viewUserInfo user)
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
            var userNumber = AppSettings.Get("Number");
            InstanceContext context = new InstanceContext(this);
            ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
            {
                net.InsertDiaryItems(diaryItems);
            });
        }

        public void ReturnDiaryItemInsertPercent(int percent)
        {
            OnDiaryItemsInsert(this, new ServiceContract.DiaryItemsInsertEventArgs(percent));
        }
    }
}