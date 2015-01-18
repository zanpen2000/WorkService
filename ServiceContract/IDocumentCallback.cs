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
        event EventHandler<UserInfoEventArgs> OnGetUserInfo;
        event EventHandler<ViewDiarysEventArgs> OnGetUserDiarys;
        event EventHandler<DiaryEventArgs> OnLoadDiary;
        event EventHandler<DiarysEventArgs> OnLoadDiarys;
        event EventHandler<DiaryItemsInsertEventArgs> OnDiaryItemsInsert;

        [OperationContract]
        void ReturnUserInfo(viewUserInfo user);

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
    }


}
