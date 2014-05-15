using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc.Html
{
    public static class WebViewPageExtensions
    {
        public static DateTime GetCurrentDateTime(this WebViewPage webViewPage)
        {
            return Sic.Web.Mvc.Session.CurrentDateTime;
        }

        public static int GetUserId(this WebViewPage webViewPage)
        {
            return Sic.Web.Mvc.Session.UserId;
        }

        public static string GetUserFullName(this WebViewPage webViewPage)
        {
            return Sic.Web.Mvc.Session.FullName;
        }

        public static bool GetIsLogged(this WebViewPage webViewPage)
        {
            return Sic.Web.Mvc.Session.IsLogged;
        }

        public static string GetUserLogonName(this WebViewPage webViewPage)
        {
            return Sic.Web.Mvc.Session.LogonName;
        }        

        public static void SetTitle(this WebViewPage webViewPage, string title)
        {
            webViewPage.ViewBag.Title = title;
        }        

        public static void SetImportFormElements(this WebViewPage webViewPage, bool value)
        {
            webViewPage.ViewBag.Rp3WebPageImportFormElements = value;
        }

        public static bool ImportFormElements(this WebViewPage webViewPage)
        {
            return Convert.ToBoolean(webViewPage.ViewBag.Rp3WebPageImportFormElements);
        }

        public static void SetImportTables(this WebViewPage webViewPage, bool value)
        {
            webViewPage.ViewBag.Rp3WebPageImportTables = value;
        }

        public static bool ImportTables(this WebViewPage webViewPage)
        {
            return Convert.ToBoolean(webViewPage.ViewBag.Rp3WebPageImportTables);
        }

        public static void SetImportFormValidations(this WebViewPage webViewPage, bool value)
        {
            webViewPage.ViewBag.Rp3WebPageImportFormValidations = value;
        }

        public static bool ImportFormValidations(this WebViewPage webViewPage)
        {
            return Convert.ToBoolean(webViewPage.ViewBag.Rp3WebPageImportFormValidations);
        }

        public static void SetInlineNotificationMessage(this WebViewPage webViewPage, bool value, bool canClose)
        {
            webViewPage.ViewBag.InlineNotificationMessage = value;
            webViewPage.ViewBag.InlineNotificationMessageCanClose = value;
        }

        public static bool InlineNotificationMessage(this WebViewPage webViewPage)
        {
            return Convert.ToBoolean(webViewPage.ViewBag.InlineNotificationMessage);
        }

        public static bool InlineNotificationMessageCanClose(this WebViewPage webViewPage)
        {
            return Convert.ToBoolean(webViewPage.ViewBag.InlineNotificationMessageCanClose);
        }       
    }
}
