using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public class LoggedRequiredAttribute: FilterAttribute, IAuthorizationFilter
    {
        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Sic.Web.Mvc.Session.IsLogged)
            {
                if (filterContext.IsChildAction || filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                        Data = new { IsRedirect = true, Url = Sic.Configuration.SicConfigurationSection.Current.AccessDeniedRedirect.Url }
                    };
                else
                    filterContext.Result = new RedirectResult(Sic.Configuration.SicConfigurationSection.Current.AccessDeniedRedirect.Url);
            }
        }

        #endregion
    }
}
