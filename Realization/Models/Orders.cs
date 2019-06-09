using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Orders
    {
        public int OrderId { set; get; }
        public string UserName { set; get; }
        public int ProductId { set; get; }

        public string ProductName { set; get; }
        public double ProductPrice { set; get; }
        public int Quantity { set; get; }

        public double AllPrice { set; get; }
        public DateTime OrderTime { set; get; }
        public string OrderStatus { set; get; } 

        public string ShopTo { set; get; }
        public int Identifier { set; get; }
    }
}