using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Reasons
    {
        public int Id { set; get; }
        public int OrderId { set; get; }
        public string UserName { set; get; }
        public string Reason { set; get; }
        public int ProductId { set; get; }
        public string ProductName { set; get; }
        public double ProductPrice { set; get; }
        public int Quantity { set; get; }
        public double AllPrice { set; get; }
        public DateTime OrderTime { set; get; }
        public string Status { set; get; }

    }
}