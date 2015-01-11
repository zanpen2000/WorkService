using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AppLayer
{
    public static class GlobalSettings
    {
        /// <summary>
        /// 本会话SessionId
        /// </summary>
        public static string LocalSessionId;

        /// <summary>
        /// 用户id
        /// </summary>
        public static string UserId;


        public static T DeepCopy<T>(this T obj)
        {
            object retval = Activator.CreateInstance(typeof(T));
            PropertyInfo[] pis = typeof(T).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                try { pi.SetValue(retval, pi.GetValue(obj, null), null); }
                catch { }
            }
            return (T)retval;
        }
    }
}
