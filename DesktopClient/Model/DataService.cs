
using DBModel;
using System;

namespace DesktopClient.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        public void ReturnUserInfo(codeUsers user)
        {
            throw new NotImplementedException();
        }

        public void ReturnUserDiarys(System.Collections.Generic.IEnumerable<viewUserDiarys> diarys)
        {
            throw new NotImplementedException();
        }
    }
}