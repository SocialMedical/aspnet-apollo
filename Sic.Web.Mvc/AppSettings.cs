using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Web.Mvc
{
    public class AppSettings
    {
        public static bool UseMultiCulture
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseMultiCulture"]);            
            }
        }
    }
}
