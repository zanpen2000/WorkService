using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract
{
    public class UserInfoEventArgs : EventArgs
    {
        public viewUserInfo UserInfo { get; private set; }

        public UserInfoEventArgs(viewUserInfo user)
        {
            UserInfo = user;
        }
    }
}
