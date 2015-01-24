using ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public interface IDataService : ServiceContract.IDocumentCallback
    {
        event EventHandler<UserInfoEventArgs> OnGetUserInfo;
        event EventHandler<ViewDiarysEventArgs> OnGetUserDiarys;
        event EventHandler<DiaryEventArgs> OnLoadDiary;
        event EventHandler<DiarysEventArgs> OnLoadDiarys;
        event EventHandler<DiaryItemsInsertEventArgs> OnDiaryItemsInsert;
        event EventHandler<RowAffectedEventArgs> OnSavedToDatabase;
        event EventHandler<ServerExcelFilenameEventArg> OnSavedExcelFile;
        event EventHandler<ServerSendDiaryEventArg> OnServerSendDiary;

        void GetUserInfo();

        void GetDiarys(string page);

        void LoadDiary(int id);

        void LoadDiaryItems(int userId, DateTime date);

        void InsertDiaryItems(IEnumerable<DBModel.domainDiary> diaryItems);

        void EditUser(DBModel.codeUsers user);

        void SendDiary(string number, DateTime date);
    }
}
