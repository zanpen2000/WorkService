
using AppLayer;
using DBModel;
using System;
using System.ServiceModel;

namespace DesktopClient.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        public void ReturnUserDiarys(System.Collections.Generic.IEnumerable<viewUserDiarys> diarys)
        {

        }

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

        public void ReturnUserInfo(codeUsers user)
        {
            OnGetUserInfo(this, new ServiceContract.UserInfo(user));
        }

        public event EventHandler<ServiceContract.UserInfo> OnGetUserInfo;
    }
}