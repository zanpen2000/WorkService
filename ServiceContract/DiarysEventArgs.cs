using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class DiarysEventArgs : EventArgs
    {
        public IEnumerable<DBModel.domainDiary> Items { get; private set; }

        public DiarysEventArgs(IEnumerable<DBModel.domainDiary> items)
        {
            this.Items = items;
        }
    }
}
