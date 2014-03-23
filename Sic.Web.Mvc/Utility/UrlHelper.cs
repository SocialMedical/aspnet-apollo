using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Utility
{
    public static class UrlHelper
    {
        //private static System.Web.Mvc.UrlHelper _urlHelper;

        public static System.Web.Mvc.UrlHelper GetFromContext()
        {
            System.Web.Mvc.UrlHelper _urlHelper;
            //if (_urlHelper == null)
            //{
                if (HttpContext.Current == null)
                {
                    throw new HttpException("Current httpcontext is null!");
                }

                if (!(HttpContext.Current.CurrentHandler is System.Web.Mvc.MvcHandler))
                {
                    throw new HttpException("Type casting is failed!");
                }
                
                _urlHelper = new System.Web.Mvc.UrlHelper(((System.Web.Mvc.MvcHandler)HttpContext.Current.CurrentHandler).RequestContext);
            //}

            return _urlHelper;
        }
    }
}
