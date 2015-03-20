using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public class ServerSendDiaryEventArg:EventArgs
    {
        public string Message { get; private set; }
        public bool Success { get; private set; }

        public ServerSendDiaryEventArg(bool ok, string msg)
        {
            this.Success = ok;
            this.Message = msg;
        }
    }
}
