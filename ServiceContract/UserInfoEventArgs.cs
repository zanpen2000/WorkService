using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class UserInfoEventArgs : EventArgs
    {
        public codeUsers UserInfo { get; private set; }

        public UserInfoEventArgs(codeUsers user)
        {
            UserInfo = user;
        }
    }
}
