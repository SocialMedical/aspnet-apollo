using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sic.Web.Mvc
{
    public class AppStart
    {
        public static void Initialize(HttpApplication app)
        {
            ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Remove(ViewEngines.Engines.First(e => e is System.Web.Mvc.RazorViewEngine));
            ViewEngines.Engines.Insert(0, new Sic.Web.Mvc.RazorViewEngine());

            
            Sic.Security.Cryptography.KeyFileName = app.Server.MapPath("~/key");
        }

    }
}
