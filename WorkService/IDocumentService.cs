using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WorkService
{

    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IDocumentService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/getdata/{value}")]
        string GetData(string value);

        [OperationContract]
        Models.ServerMessage RegisNewUser(Model.codeUsers user);

        [OperationContract(IsInitiating = true)]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "login/{user}/{pwd}")]
        Models.ServerMessage Login(string user, string pwd);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "adddiary/{title}/{content}")]
        Models.ServerMessage AddDiary(int userId, string title, string content);

        [OperationContract(IsInitiating = true)]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getresult")]
        string GetResult();

        [OperationContract(IsInitiating = true)]
        void SetResult(int value);

        [OperationContract(IsInitiating = true)]
        string GetSessionId();

    }
}
