using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public class AuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {        
        protected string AccessDeniedPage {
            get
            {
                return Sic.Configuration.SicConfigurationSection.Current.AccessDeniedRedirect.Url;
            }
        }        

        protected string ExpiredSessionPage
        {
            get
            {
                return Sic.Configuration.SicConfigurationSection.Current.ExpiredSessionRedirect.Url;
            }
        }

        public int UserId
        {
            get
            {
                return Sic.Web.Mvc.Session.UserId;
            }
        }

        public int UserType
        {
            get
            {
                return Sic.Web.Mvc.Session.UserType;
            }
        }        

        public bool IsLogged
        {
            get
            {
                return Sic.Web.Mvc.Session.IsLogged;
            }
        }

        public bool IsValid
        {
            get;
            set;
        }

        #region IAuthorizationFilter Members

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                Sic.Web.Mvc.Session.UrlSecureLastAttempted = filterContext.HttpContext.Request.Url.PathAndQuery;

            if (!Sic.Web.Mvc.Session.IsLogged)
            {
                IsValid = false;
                string url = string.Empty;
                if (filterContext.HttpContext.Request.IsAjaxRequest())//filterContext.HttpContext.Request.HttpMethod == "POST")
                     url = ExpiredSessionPage;                
                else                
                    url = AccessDeniedPage;

                if (filterContext.IsChildAction || filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new JsonResult() { JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                        Data = new { IsRedirect = true, Url = url } };
                else
                    filterContext.Result = new RedirectResult(url);
            }
            else
            {
                IsValid = true;
            }
        }

        #endregion
    }
}
