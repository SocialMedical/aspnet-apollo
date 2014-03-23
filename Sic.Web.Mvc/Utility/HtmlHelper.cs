using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rp3.Web.Mvc.Utility
{
    public static class HtmlHelper
    {        
        public static IDictionary<string, object> MergeAnonymousObjectHtmlAttributes(object htmlAttributes1, object htmlAttributes2)
        {            
            RouteValueDictionary dictionary1 = null;
            RouteValueDictionary dictionary2 = null;

            if(htmlAttributes1 is IDictionary<string,object>)            
                dictionary1 = htmlAttributes1 as RouteValueDictionary;            
            else
                dictionary1 = htmlAttributes1 != null ?
                    System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes1) : new RouteValueDictionary();

            if(htmlAttributes2 is IDictionary<string,object>)            
                dictionary2 = htmlAttributes2 as RouteValueDictionary;            
            else
                dictionary2 = htmlAttributes2 != null ?
                    System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes2) : new RouteValueDictionary();

            if (htmlAttributes2 != null)
            {
                foreach (var attr in dictionary2)
                {
                    if (!dictionary1.ContainsKey(attr.Key))
                    {
                        dictionary1.Add(attr.Key, attr.Value);
                    }
                    else if (attr.Key == "style")
                    {
                        dictionary1["style"] += "; " + Convert.ToString(attr.Value);
                    }
                    else if (attr.Key == "class")
                    {
                        dictionary1["class"] += " " + Convert.ToString(attr.Value);
                    }
                }
                //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes2))
                //{
                //    var key = property.Name.Replace('_', '-');
                //    if (key != "Keys" && key != "Count" && key != "Values")
                //    {
                //        if (!dictionary1.ContainsKey(key))
                //        {
                //            dictionary1.Add(key, property.GetValue(htmlAttributes2));
                //        }
                //        else if (key == "class")
                //        {
                //            dictionary1["class"] += " " + Convert.ToString(property.GetValue(htmlAttributes2));
                //        }
                //    }
                //}                
            }

            return dictionary1;
        }
    }
}
