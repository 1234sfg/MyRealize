using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Stocks
    {
        public int Id { set; get; }
        public int ProductId { set; get; }
        public string ProductModel { set; get; } 
        public int ProductAmount { set; get; }
    }
}