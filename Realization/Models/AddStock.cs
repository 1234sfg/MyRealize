using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class AddStock
    {
        public int Id { set; get; }
        public int ProductId { set; get; }
        public int Amount { set; get; }
        public DateTime Date { set; get; }
    }
}