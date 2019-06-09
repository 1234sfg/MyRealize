using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Products
    {
        public int ProductId { set; get; }
        public string ProductType { set; get; }
        public string ProductBrand { set; get; }
        public string ProductName { set; get; }
        public double ProductPriceIn { set; get; }
        public double ProductPriceOut { set; get; }
        public string ProductModel { set; get; }
        public string ProductDetail { set; get; }
        public string ProductPath { set; get; }
        public string ProviderName { set; get; }
        public int Identifier { set; get; }

    }
    }