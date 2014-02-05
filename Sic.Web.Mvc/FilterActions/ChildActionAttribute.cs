using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public class ChildActionAttribute : FilterAttribute, IAuthorizationFilter
    {               
        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
             if (!(filterContext.IsChildAction || filterContext.HttpContext.Request.IsAjaxRequest()))
                 filterContext.Result = new RedirectResult("/");                
        }

        #endregion
    }
}