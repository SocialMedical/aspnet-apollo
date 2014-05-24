﻿using Sic.Web.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public class PreventSpamAttribute : FilterAttribute, IAuthorizationFilter
    {
        public PreventSpamAttribute()
            : this(null, 10)
        {            
        }

        public PreventSpamAttribute(int delayRequest)
            : this("", 10)
        {
        }

        public PreventSpamAttribute(string redirectURL)
            :this(redirectURL,10)
        {
        }

        public PreventSpamAttribute(string redirectURL,int delayRequest)
        {
            DelayRequest = delayRequest;
            RedirectURL = redirectURL;
        }

        //This stores the time between Requests (in seconds)
        public int DelayRequest = 10;
        //The Error Message that will be displayed in case of excessive Requests
        public string ErrorMessage = Sic.Resources.MessageFor.DefaultErrorPreventSpamMessage;
        //This will store the URL to Redirect errors to
        public string RedirectURL = "";       

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //Store our HttpContext (for easier reference and code brevity)
            var request = filterContext.HttpContext.Request;
            //Store our HttpContext.Cache (for easier reference and code brevity)
            var cache = filterContext.HttpContext.Cache;

            //Grab the IP Address from the originating Request (very simple implementation for example purposes)
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            //Append the User Agent
            originationInfo += request.UserAgent;

            //Now we just need the target URL Information
            var targetInfo = request.RawUrl + request.QueryString;

            //Generate a hash for your strings (this appends each of the bytes of the value into a single hashed string
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            //Checks if the hashed value is contained in the Cache (indicating a repeat request)
            if (cache[hashValue] != null)
            {
                //Adds the Error Message to the Model and Redirect
                ((Sic.Web.Mvc.Controllers.Controller)filterContext.Controller).AddMessage(ErrorMessage, Data.MessageType.Error);
                if (!string.IsNullOrEmpty(RedirectURL))
                    filterContext.Result = new RedirectResult(RedirectURL);
            }
            else
            {
                //Adds an empty object to the cache using the hashValue to a key (This sets the expiration that will determine
                //if the Request is valid or not
                cache.Add(hashValue, hashValue, null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }           
        }
    }
}
