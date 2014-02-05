using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sic.Constants
{
    public class Session
    {
        public const string UserType = "UserType";
        public const string UserState = "UserState";
        public const string LogonName = "LogonName";
        public const string UserId = "UserId";
        public const string UserFullName = "UserFullName";
        public const string IsLogged = "IsLogged";
        public const string UrlSecureLastAttempted = "UrlSecureLastAttempted";
    }

    public class MessageType
    {
        public const string Success = "success";
        public const string Error = "error";
        public const string Warning = "warning";
        public const string Information = "information";
    }
}

namespace Sic.Web.Mvc
{        
    public class Session
    {
        public static bool IsLogged
        {
            get
            {
                return Convert.ToBoolean(GetValue(Sic.Constants.Session.IsLogged));
            }
            set
            {
                SetValue(Sic.Constants.Session.IsLogged, value);
            }
        }

        public static int UserId
        {
            get
            {
                return Convert.ToInt32(GetValue(Sic.Constants.Session.UserId));
            }
            set
            {
                SetValue(Sic.Constants.Session.UserId, value);
            }
        }

        public static string LogonName
        {
            get
            {
                return Convert.ToString(GetValue(Sic.Constants.Session.LogonName));
            }
            set
            {
                SetValue(Sic.Constants.Session.LogonName, value);
            }
        }

        public static int UserState
        {
            get
            {
                return Convert.ToInt32(GetValue(Sic.Constants.Session.UserState));
            }
            set
            {
                SetValue(Sic.Constants.Session.UserState, value);
            }
        }

        public static int UserType
        {
            get
            {
                return Convert.ToInt32(GetValue(Sic.Constants.Session.UserType));
            }
            set
            {
                SetValue(Sic.Constants.Session.UserType, value);
            }
        }

        public static string FullName
        {
            get
            {
                return Convert.ToString(GetValue(Sic.Constants.Session.UserFullName));
            }
            set
            {
                SetValue(Sic.Constants.Session.UserFullName, value);
            }
        }

        public static string UrlSecureLastAttempted
        {
            get
            {
                return Convert.ToString(GetValue(Sic.Constants.Session.UrlSecureLastAttempted));
            }
            set
            {
                SetValue(Sic.Constants.Session.UrlSecureLastAttempted, value);
            }
        }        

        public static DateTime CurrentDateTime
        {
            get
            {
                return Sic.Runtime.Current.GetCurrentDateTime();
            }
        }

        public static double DefaultLatitude
        {
            get { return -2.139343; }
        }

        public static double DefaultLongitude
        {
            get { return -79.90108; }
        }

        public static void SetValue(string key, object value)
        {   
            if(HttpContext.Current.Session != null)
                HttpContext.Current.Session[key] = value;
        }

        public static object GetValue(string key)
        {
            if (HttpContext.Current.Session != null)
                return HttpContext.Current.Session[key];
            else
                return null;
        }
    }
}
