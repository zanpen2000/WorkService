using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class ViewDiarysEventArgs : EventArgs
    {
        public IEnumerable<viewUserDiarys> Diarys { get; private set; }

        public ViewDiarysEventArgs(IEnumerable<viewUserDiarys> diarys)
        {
            Diarys = diarys;
        }
    }
}
