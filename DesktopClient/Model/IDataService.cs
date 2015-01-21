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
        event EventHandler<RowAffectedEventArgs> OnSaved;

        void GetUserInfo();

        void GetDiarys(string page);

        void LoadDiary(int id);

        void LoadDiaryItems(int userId, DateTime date);

        void InsertDiaryItems(IEnumerable<DBModel.domainDiary> diaryItems);

        void InsertUser(DBModel.codeUsers user);
    }
}
