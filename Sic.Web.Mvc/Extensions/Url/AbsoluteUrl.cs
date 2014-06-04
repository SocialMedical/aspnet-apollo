using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sic.Web.Mvc
{
    public static class AbsoluteUrlExtensions
    {
        public static string AbsoluteUrlImage(this System.Web.Mvc.UrlHelper helper, string imageName)
        {
            return helper.AbsoluteUrl(UrlContentExtensions.CombinePath(Sic.Configuration.SicConfigurationSection.Current.Image.Path.Replace("~",""), imageName));
        }

        public static string AbsoluteUrl(this System.Web.Mvc.UrlHelper helper, string contentPath = null)
        {
            return string.Format("{0}://{1}{2}", helper.RequestContext.HttpContext.Request.Url.Scheme, helper.RequestContext.HttpContext.Request.Url.Authority,
                contentPath);
        }

        public static string AbsoluteAction(this System.Web.Mvc.UrlHelper helper, string action, string controller = null, object routeValues = null)
        {
            return string.Format("{0}://{1}{2}", helper.RequestContext.HttpContext.Request.Url.Scheme, helper.RequestContext.HttpContext.Request.Url.Authority,
                helper.Action(action, controller, routeValues));
        }
    }
}
