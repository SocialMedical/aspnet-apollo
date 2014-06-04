using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Web.Mvc
{
    public static class JScriptUrlExtensions
    {
        public static string ToUrlStringParameter(this string parameter)
        {
            return parameter.Replace(" ", "-").Replace(".", "_");
        }

        public static string ToOriginalUrlStringParameter(this string parameter)
        {
            return parameter.Replace("-", " ").Replace("_", ".");
        }
    }
}
