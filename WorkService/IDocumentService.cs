using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WorkService
{
    
    [ServiceContract]
    public interface IDocumentService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/getdata/{value}")]
        string GetData(string value);

        [OperationContract]
        Models.ServerMessage RegisNewUser(Model.codeUsers user);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "login/{user}/{pwd}")]
        Models.ServerMessage Login(string user, string pwd); 
    }
}
