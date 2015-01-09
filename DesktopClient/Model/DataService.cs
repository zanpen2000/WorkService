
using AppLayer;
using DBModel;
using System;
using System.ServiceModel;

namespace DesktopClient.Model
{
    public class DataService : IDataService
    {
        public event EventHandler<ServiceContract.UserInfoEventArgs> OnGetUserInfo;
        public event EventHandler<ServiceContract.DiarysEventArgs> OnGetUserDiarys;
        

        public void GetUserInfo()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var userNumber = AppSettings.Get("Number");
                InstanceContext context = new InstanceContext(this);
                ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
                {
                    net.GetUserInfo(userNumber);
                });
            });
        }

        public void ReturnUserInfo(viewUserInfo user)
        {
            OnGetUserInfo(this, new ServiceContract.UserInfoEventArgs(user));
        }

        public void GetDiarys(string page)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var userNumber = AppSettings.Get("Number");
                InstanceContext context = new InstanceContext(this);
                ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
                {
                    net.GetUserDiarys(userNumber, page);
                });
            });
        }
        public void ReturnUserDiarys(System.Collections.Generic.IEnumerable<viewUserDiarys> diarys)
        {
            OnGetUserDiarys(this, new ServiceContract.DiarysEventArgs(diarys));
        }

        public void LoadDiary(int id)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var userNumber = AppSettings.Get("Number");
                InstanceContext context = new InstanceContext(this);
                ServiceCaller.Execute<ServiceContract.IDocumentService>(context, net =>
                {
                    net.LoadDiary(id);
                });
            });
        }


        public event EventHandler<ServiceContract.DiaryEventArgs> OnLoadDiary;

        public void ReturnUserDiary(domainDiary diary)
        {
            OnLoadDiary(this, new ServiceContract.DiaryEventArgs(diary));
        }
    }
}