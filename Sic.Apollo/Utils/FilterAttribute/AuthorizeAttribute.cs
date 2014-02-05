using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Apollo
{
    public class AuthorizeAttribute : Sic.Web.Mvc.AuthorizeAttribute
    {
        public AuthorizeAttribute(params UserType[] UserRole)
        {
            userTypes = new List<UserType>();
            userTypes.AddRange(UserRole);          

            if (UserRole.Contains(Sic.UserType.Administrator) && !UserRole.Contains(Sic.UserType.Professional))
                userTypes.Add(Sic.UserType.Professional);

            if ((UserType)Sic.Web.Mvc.Session.UserType == Sic.UserType.Professional
                && userTypes.Contains(Sic.UserType.Customer) && !userTypes.Contains(Sic.UserType.Professional))
                userTypes.Add(Sic.UserType.Professional);

            if ((UserType)Sic.Web.Mvc.Session.UserType == Sic.UserType.Assistant
                && userTypes.Contains(Sic.UserType.Customer) && !userTypes.Contains(Sic.UserType.Assistant))
                userTypes.Add(Sic.UserType.Assistant);
        }        

        private readonly List<UserType> userTypes;

        #region IAuthorizationFilter Members

        public override void OnAuthorization(AuthorizationContext filterContext)
        {      
            base.OnAuthorization(filterContext);

            if (base.IsValid)
            {                
                if (!Sic.Web.Mvc.Session.IsLogged ||
                    !userTypes.Contains((UserType)Sic.Web.Mvc.Session.UserType))
                {
                    if (filterContext.IsChildAction || filterContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.Result = new JsonResult()
                        {
                            JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                            Data = new { IsRedirect = true, Url = AccessDeniedPage }
                        };
                    else
                        filterContext.Result = new RedirectResult(AccessDeniedPage);
                }
            }
        }

        #endregion
    }
}
