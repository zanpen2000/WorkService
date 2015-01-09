using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class DiaryEventArgs : EventArgs
    {
        public DBModel.domainDiary Diary { get; private set; }

        public DiaryEventArgs(DBModel.domainDiary diary)
        {
            this.Diary = diary;
        }
    }
}
