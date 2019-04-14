using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Realization.Models
{
    public  static class Dbconn
    {
        public static SqlConnection getConnn() {
             return new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ConnectionString);
        } 
    }
}