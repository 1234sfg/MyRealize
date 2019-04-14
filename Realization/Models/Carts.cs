using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Carts
    {
        public int Id { set; get; }
        public string UserName { set; get; }
        public int ProductId { set; get; }
        public string ProductName { set; get; }
        public double ProductPrice { set; get; }
        public int Quantity { set; get; }
    }
}