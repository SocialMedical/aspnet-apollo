using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public class RazorViewEngine : System.Web.Mvc.RazorViewEngine
    {
        public RazorViewEngine()
            : this( null )
        {
        }

        public RazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            ViewLocationFormats = ViewLocationFormats
                .Union(new[] { "~/Areas/Base/Views/Shared/{0}.cshtml", "~/Areas/Security/Views/Shared/{0}.cshtml", "~/Areas/Security/Views/Account/{0}.cshtml", "~/Areas/Security/Views/Mail/{0}.cshtml" })
                .ToArray();

            PartialViewLocationFormats = ViewLocationFormats;

            AreaViewLocationFormats = AreaViewLocationFormats
                .Union(new[] { "~/Areas/Base/Views/Shared/{0}.cshtml", "~/Areas/Security/Views/Shared/{0}.cshtml", "~/Areas/Security/Views/Account/{0}.cshtml", "~/Areas/Security/Views/Mail/{0}.cshtml" })
                .ToArray();

            AreaPartialViewLocationFormats = AreaViewLocationFormats;
        }
    }
}
