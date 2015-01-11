using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public interface IDataService : ServiceContract.IDocumentCallback
    {
        void GetUserInfo();

        void GetDiarys(string page);

        void LoadDiary(int id);

        void LoadDiaryItems(int userId, DateTime date);

        void InsertDiaryItems(IEnumerable<DBModel.domainDiary> diaryItems);
    }
}
