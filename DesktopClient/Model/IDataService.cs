using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public interface IDataService : ServiceContract.IDocumentCallback
    {
        void GetData(Action<DataItem, Exception> callback);


        void GetUserInfo();


    }
}
