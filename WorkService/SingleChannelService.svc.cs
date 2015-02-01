using ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WorkService
{
    using DBModel;

    public partial class ServiceImpl : ISingleChannelService
    {

        public string GetFilePathById(int fileId)
        {
            domainFiles df = new domainFiles();
            return df.Where(d => d.id == fileId).Select().filepath;
        }
    }
}
