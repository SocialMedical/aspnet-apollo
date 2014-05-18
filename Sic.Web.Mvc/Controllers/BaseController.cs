using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using Sic.Web.Mvc.Utility;
using Sic.Data.Entity;
using System.Web.Caching;
using Sic.Data;
using Sic.Data.DbConnection;
using System.Text;
using Sic.Web.Mvc.Models;

namespace Sic.Web.Mvc.Controllers
{
    public abstract class Controller : System.Web.Mvc.Controller
    {
        protected string UserLogonName
        {
            get
            {
                return Sic.Web.Mvc.Session.LogonName;
            }
        }

        protected string UserFullName
        {
            get
            {
                return Sic.Web.Mvc.Session.FullName;
            }
        }

        protected int UserId
        {
            get
            {
                return Sic.Web.Mvc.Session.UserId;
            }
        }

        protected bool IsLogged
        {
            get
            {
                return Sic.Web.Mvc.Session.IsLogged;
            }
        }

        protected string UrlSecureLastAttempted
        {
            get
            {
                return Sic.Web.Mvc.Session.UrlSecureLastAttempted;
            }
        }

        protected DateTime GetCurrentDateTime()
        {
            return Sic.Runtime.Current.GetCurrentDateTime();
        }

        protected JsonResult Json()
        {
            return base.Json(
                new JsonData() { Messages = Messages });
        }

        protected new JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            object Content = data;
            return base.Json(
                new JsonData() { Messages = Messages, Content = data }, behavior);
        }

        protected new JsonResult Json(object data)
        {                                 
            return base.Json(new JsonData() { Messages = Messages, Content = data });            
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {            
            return base.Json(new JsonData() { Messages = Messages, Content = data },
                contentType, contentEncoding);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return base.Json(new JsonData() { Messages = Messages, Content = data },
                contentType, contentEncoding, behavior);
        }

        protected WrappedJsonResult WrappedJson()
        {            
            return new WrappedJsonResult()
            {
                Data = new JsonData() { Messages = Messages }
            };
        }

        protected WrappedJsonResult WrappedJson(object data)
        {            
            return new WrappedJsonResult()
            {
                Data = new JsonData() { Messages = Messages, Content = data }
            };
        }

        private bool IsSharingMessages = false;

        private Data.MessageCollection messages;
        public Data.MessageCollection Messages
        {
            get
            {
                if (messages == null)
                    messages = new Data.MessageCollection();
                return messages;
            }
        }


        protected new RedirectToRouteResult RedirectToAction(string actionName, object routeValues)
        {
            if (this.Messages.Any()) SharedActionMessage();
            return base.RedirectToAction(actionName, routeValues);
        }

        protected new RedirectToRouteResult RedirectToAction(string actionName, string controller)
        {
            if (this.Messages.Any()) SharedActionMessage();
            return base.RedirectToAction(actionName, controller);
        }

        protected new RedirectToRouteResult RedirectToAction(string actionName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            if (this.Messages.Any()) SharedActionMessage();
            return base.RedirectToAction(actionName, routeValues);
        }

        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            if (this.Messages.Any()) SharedActionMessage();
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }

        protected RedirectToRouteResult RedirectToAction(string actionName, string controllerName, object routeValues, bool sharedMessage)
        {
            if (this.Messages.Any()) SharedActionMessage();
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }

        protected void SharedActionMessage()
        {
            if (this.Messages.Any())
            {
                var cache = this.HttpContext.Cache;
                if (cache["sharedMessages"] == null)
                    cache.Add("sharedMessages", this.Messages, null, DateTime.Now.AddSeconds(30), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                else
                    cache["sharedMessages"] = this.Messages;

                IsSharingMessages = true;
            }
        }

        private void AddSharedActionMessage()
        {
            var cache = this.HttpContext.Cache;
            if (cache["sharedMessages"] != null)
            {
                MessageCollection sharedMessageCollection = (MessageCollection)cache["sharedMessages"];
                if (sharedMessageCollection.Any())
                    this.Messages.InsertRange(0, sharedMessageCollection);
                cache.Remove("sharedMessages");
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!this.IsSharingMessages)
                AddSharedActionMessage();

            ViewBag.MessageCollection = this.Messages;

            // Is it View ?
            ViewResultBase view = filterContext.Result as ViewResultBase;
            if (view == null) // if not exit
                return;

            string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US" // filterContext.HttpContext.Request.UserLanguages[0]; // needs validation return "en-us" as default            

            // Is it default culture? exit
            if (cultureName == CultureHelper.GetDefaultCulture())
                return;


            // Are views implemented separately for this culture?  if not exit
            bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
            if (viewImplemented == false)
                return;

            string viewName = view.ViewName;

            int i = 0;

            if (string.IsNullOrEmpty(viewName))
                viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            else if ((i = viewName.IndexOf('.')) > 0)
            {
                // contains . like "Index.cshtml"                
                viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            }
            else
                viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            view.ViewName = viewName;

            filterContext.Controller.ViewBag._culture = "." + cultureName;

            base.OnActionExecuted(filterContext);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = requestContext.HttpContext.Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
            {
                //if(Request.UserLanguages!=null)
                cultureName = requestContext.HttpContext.Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
            }

            //if(!string.IsNullOrEmpty(cultureName))
            //{
            // Validate culture name
            cultureName = CultureHelper.GetValidCulture(cultureName); // This is safe

            // Modify current thread's culture            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
            //}

            base.Initialize(requestContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            this.AddMessage(filterContext.Exception.Message, Data.MessageType.Error);
            base.OnException(filterContext);
        }

        public void AddDefaultErrorMessage()
        {
            this.AddMessage(Sic.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType.Error, Sic.Resources.MessageFor.DefaultTitleErrorTransactionMessage);
        }

        public void AddDefaultSuccessMessage()
        {
            this.AddMessage(Sic.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType.Success, Sic.Resources.MessageFor.DefaultTitleSuccessTransactionMessage);
        }

        public void AddMessage(Sic.Data.Message message)
        {
            Messages.Add(message);
        }

        public void AddMessage(string message)
        {
            Messages.Add(new Data.Message(message));
        }

        public void AddMessage(string message, Sic.Data.MessageType type)
        {
            Messages.Add(new Data.Message(message, type));
        }

        public void AddMessage(string message, Sic.Data.MessageType type, string title)
        {
            Messages.Add(new Data.Message(message, type, title));
        }

        public void AddErrorMessage(string message)
        {
            Messages.Add(new Data.Message(message, MessageType.Error));
        }

        public void AddInformationMessage(string message)
        {
            Messages.Add(new Data.Message(message, MessageType.Information));
        }

        public void AddSuccessMessage(string message)
        {
            Messages.Add(new Data.Message(message, MessageType.Success));
        }
        public void AddWarningMessage(string message)
        {
            Messages.Add(new Data.Message(message, MessageType.Warning));
        }
        public void AddConfirmationMessage(string message)
        {
            Messages.Add(new Data.Message(message, MessageType.Confirmation));
        }

        public void ClearMessages()
        {
            Messages.Clear();
        }


        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        public object CopyTo(IIdentifiable entitySource, IEnumerable<IIdentifiable> findTargetCollection,
            string[] includeProperties = null, string[] excludeProperties = null)
        {
            return Sic.Data.Service.CopyTo(entitySource, findTargetCollection, includeProperties, excludeProperties);
        }

        public bool CopyTo(object source, object target,
            bool includeCollectionProperties = false, string[] includeProperties = null, string[] excludeProperties = null)
        {
            return Sic.Data.Service.CopyTo(source, target, includeCollectionProperties, includeProperties, excludeProperties);
        }
    }

    public class BaseController<T> : Controller where T : IDbContextService
    {
        private T db;
        protected virtual T DataBase
        {
            get
            {
                if (db == null)
                {                   
                    db = (T)Activator.CreateInstance(typeof(T));
                }
                return db;
            }
        }

        public void ReinitializeDataBase()
        {
            db.Dispose();
            db = default(T);
        }            

    }

    public class BaseController : BaseController<DbContextManagerService>
    {
    }
}
