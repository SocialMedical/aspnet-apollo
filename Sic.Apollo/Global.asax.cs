using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;
using System.Web.Optimization;
using System.Web.Http;

namespace Sic.Apollo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {        

        protected void Application_Start()
        {
            Sic.Security.Cryptography.KeyFileName = Server.MapPath("~/key");

            if (!Sic.Web.Mvc.AppSettings.UseMultiCulture)
            {
                Sic.Web.Mvc.Utility.CultureHelper.SetDefaultCulture();
            }

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();
        }        
    }
}