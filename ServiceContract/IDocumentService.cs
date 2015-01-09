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
        Models.ServerMessage RegisNewUser(DBModel.codeUsers user);

        [OperationContract(IsInitiating = true)]
        Models.ServerMessage Login(string user, string pwd);

        [OperationContract]
        Models.ServerMessage AddDiary(int userId, string title, string content);

        [OperationContract]
        void GetUserInfo(string number);

        [OperationContract]
        void GetUserDiarys(string userNum, string currentpage);

         [OperationContract]
        void LoadDiary(int id);
    }
}
