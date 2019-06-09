using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Users
    {
        public int Id { set; get; }
        public string UserName { set; get; }
        public string UserPassword { set; get; }
        public string UserEmail { set; get; }
        public string UserPhone { set; get; }
        public int Identifier { set; get; }
    }
}