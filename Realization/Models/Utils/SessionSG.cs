using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{   
    /// <summary>
    /// sesseion操作1
    /// </summary>
    public class SessionSG
    {    // 根据session名获取session对象
        public static object GetSession(string name)
        {
            return HttpContext.Current.Session[name];
        }
        // 设置session
        public static void SetSession(string name, object val)
        {
            HttpContext.Current.Session.Remove(name);//先移除原先有的名
            HttpContext.Current.Session.Add(name, val);//设置新的session名和值
        }
        //移除session
        public static void RemoveSession(string name)
        {
            HttpContext.Current.Session.Remove(name);//页面上设置的session一般是用户Users
        }
    }
}