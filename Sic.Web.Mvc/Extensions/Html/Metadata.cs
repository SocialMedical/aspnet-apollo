using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public static class MetadataExtensions
    {
        public static MvcHtmlString MetaDescription(this HtmlHelper helper, string content)
        {
            return MvcHtmlString.Create(string.Format("<meta name=\"Description\" content=\"{0}\">", content));
        }

        public static MvcHtmlString MetaRobots(this HtmlHelper helper, string content)
        {
            return MvcHtmlString.Create(string.Format("<meta name=\"robots\" content=\"{0}\"/>", content));
        }
    }
}
