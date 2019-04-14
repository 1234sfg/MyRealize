using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class DOders
    {
        public int Id { set; get; }
        public int OrderId { set; get; }
        public string UserName { set; get; }
        public int ProductId { set; get; }
        public string ProdutcName { set; get; }
        public double ProductPrice { set; get; }
        public int Quantity { set; get; }
        /// <summary>
        /// sql中 yyyy-MM-dd HH:mm:ss.fff
        /// C#中 DateTime.Now.ToString()
        /// </summary>
        public DateTime OrderTime { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public string Address { set; get; }
        public double AllPrice { set; get; }
    }
}