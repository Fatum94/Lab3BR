using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.ProviderBase;
using System.Configuration;

namespace Registration.Models
{
     public class DefaultConnection : System.Web.UI.Page
    {
         SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
         protected void Page_Load()
         {
             connection.Open();
         }

    }

   
}
