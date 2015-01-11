using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DBHelper;

using ServiceContract;
using ServiceContract.Models;
using DBModel;

namespace WorkService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DocumentService : IDocumentService
    {


        public void GetUserDiarys(string userNum, string currentpage)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            viewUserDiarys diarys = new viewUserDiarys();
            diarys.Where(w => w.number == userNum);
            PagerList<viewUserDiarys> ds = diarys.SelectPageList(currentpage, "20", "lastsent", "0", "number");
            callback.ReturnUserDiarys(ds);
        }

        public void GetUserInfo(string number)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            viewUserInfo user = new viewUserInfo().Select(u => u.number == number);
            callback.ReturnUserInfo(user);
        }


        public void LoadDiary(int id)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.id == id);
            var ds = diarys.Select();
            callback.ReturnUserDiary(ds);
        }

        public void LoadDiarys(int userid, DateTime date)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();
            domainDiary diarys = new domainDiary();
            diarys.Where(w => w.userId == userid && w.date == date);
            var ds = diarys.SelectList();
            callback.ReturnDiaryItems(ds);
        }

        public void InsertDiaryItems(IEnumerable<domainDiary> diaryItems)
        {
            IDocumentCallback callback = OperationContext.Current.GetCallbackChannel<IDocumentCallback>();

            int i = 0;
            foreach (var item in diaryItems)
            {
                item.Insert();
                i++;
                int percent = (int)(((double)i / (double)((long)diaryItems.Count())) * 100);
                callback.ReturnDiaryItemInsertPercent(percent);
            }
        }
    }

}
