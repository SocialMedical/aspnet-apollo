using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web;

namespace Sic.Web.Mvc.Serialization
{
    public class JavaScript
    {
        public static HtmlString Encode(object obj)
        {
            return new HtmlString(new JavaScriptSerializer().Serialize(obj.ToString()));
        }
    }
}
