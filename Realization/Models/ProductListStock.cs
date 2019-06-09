using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    /// <summary>
    /// 从库存和商品列表，双表关联，查询数据。
    /// </summary>
    public class ProductListStock
    {
        public int ProductId { set; get; }
        public string ProductType { set; get; }
        public string ProductBrand { set; get; }
        public string ProductName { set; get; } 
        public double ProductPriceOut { set; get; }
        public string ProductModel { set; get; }
        public string ProductDetail { set; get; }
        public string ProductPath { set; get; }
        public int ProductAmount { set; get; }
    }
}