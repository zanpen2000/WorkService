using System;
using DesktopClient.Model;

namespace DesktopClient.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }

        public void ReturnUserInfo(DBModel.codeUsers user)
        {
            throw new NotImplementedException();
        }

        public void ReturnUserDiarys(System.Collections.Generic.IEnumerable<DBModel.viewUserDiarys> diarys)
        {
            throw new NotImplementedException();
        }
    }
}