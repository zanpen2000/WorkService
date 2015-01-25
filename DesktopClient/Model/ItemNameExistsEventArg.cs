using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public class ItemNameExistsEventArg:EventArgs
    {
        public bool Exists { get; private set; }
        public ItemNameExistsEventArg(bool exists)
        {
            this.Exists = exists;
        }
    }
}
