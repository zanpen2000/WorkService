using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

using System.Text;

namespace ServiceContract
{

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDocumentCallback))]
    public interface IDocumentService
    {
        [OperationContract]
        string GetData(string value);

        [OperationContract]
        Models.ServerMessage RegisNewUser(Model.codeUsers user);

        [OperationContract(IsInitiating = true)]
        Models.ServerMessage Login(string user, string pwd);

        [OperationContract]
        Models.ServerMessage AddDiary(int userId, string title, string content);

        [OperationContract(IsInitiating = true)]
        string GetResult();

        [OperationContract(IsInitiating = true)]
        void SetResult(int value);

        [OperationContract]
        void GetUserInfo(string number);

        [OperationContract]
        void GetUserDiarys(string userNum, string currentpage);


        
    }
}
