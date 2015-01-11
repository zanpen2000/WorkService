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
        void GetUserInfo(string number);

        [OperationContract]
        void GetUserDiarys(string userNum, string currentpage);

        [OperationContract]
        void LoadDiary(int id);
        [OperationContract]
        void LoadDiarys(int userid, DateTime date);

        [OperationContract]
        void InsertDiaryItems(IEnumerable<DBModel.domainDiary> diaryItems);
    }
}
