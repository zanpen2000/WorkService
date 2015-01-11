using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class DiaryItemsInsertEventArgs : EventArgs
    {
        public int Percent { get; private set; }

        public DiaryItemsInsertEventArgs(int percent)
        {
            this.Percent = percent;
        }
    }
}
