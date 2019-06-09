using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public class Providers
    {
        public int ProviderId { set; get; }
        public string ProviderName { set; get; }
        public string ProviderPhone { set; get; }
        public string ProviderAddress { set; get; }
        public int Identifier { set; get; }
    }
}