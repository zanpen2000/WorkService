using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ServiceContract
{
    public interface IDocumentCallback
    {
      
        

        [OperationContract]
        void ReturnUserInfo(codeUsers user);

        [OperationContract]
        void ReturnUserDiarys(IEnumerable<DBModel.viewUserDiarys> diarys);

        [OperationContract]
        void ReturnUserDiary(DBModel.domainDiary diary);

        [OperationContract]
        void ReturnDiaryItems(IEnumerable<DBModel.domainDiary> items);

        [OperationContract]
        void ReturnDiaryItemInsertPercent(int percent);

        [OperationContract]
        void ReturnSendDiary(bool successed, string msg);

        [OperationContract]
        void ReturnRowAffected(int r);
    }


}
