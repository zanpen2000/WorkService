using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class RowAffectedEventArgs:EventArgs
    {
        public int RowAffected { get; private set; }
        public RowAffectedEventArgs(int r)
        {
            this.RowAffected = r;
        }
    }
}
