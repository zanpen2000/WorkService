using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ISingleChannelService
    {

        [OperationContract]
        string GetFilePathById(int fileId);
    }
}
