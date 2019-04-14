using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Addresses
    {
        //Id 
        public int Id { set; get; }
        //用户姓名，用于查询
        public string UserName { set; get; }
        //收件人姓名
        public string Name { set; get; }
        //收件人电话
        public string Phone { set; get; }
        //收件人地址
        public string Address { set; get; }
    }
}