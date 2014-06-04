using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Web.Mvc
{
    public static class JScriptNumericExtensions
    {
        public static string ToJSNumericValue(this object value)
        {
            return value.ToString().Replace(",", ".");
        }
    }
}
