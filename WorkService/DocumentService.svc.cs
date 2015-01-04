using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DBHelper;

namespace WorkService
{
    public class DocumentService : IDocumentService
    {


        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public Models.ServerMessage RegisNewUser(Model.codeUsers user)
        {
            if (!UserExists(user.loginId))
            {
                
            }
        }

        public Models.ServerMessage Login(string user, string pwd)
        {
            throw new NotImplementedException();
        }

        private bool UserExists(string loginId)
        {
            string sql = "select count(1) from codeUsers where loginId = '{0}'";
            int count = (int)MyDBHelper.QueryScalar(string.Format(sql, loginId));
            return count > 0;
        }
    }
}
