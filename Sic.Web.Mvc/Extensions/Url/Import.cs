using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public static class ImportExtensions
    {
        public static MvcHtmlString ImportScript(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion = false, string area = null)
        {
            return MvcHtmlString.Create(string.Format("<script src=\"{0}\"></script>", helper.ContentScript(contentPath, includeVersion, area)));
        }

        public static MvcHtmlString ImportStyleSheet(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion = false, string styleClass = null, string area = null)
        {
            string resultClass = string.Empty;
            if (!string.IsNullOrWhiteSpace(styleClass))
                resultClass = string.Format(" class=\"{0}\"",styleClass);

            return MvcHtmlString.Create(string.Format("<link rel=\"stylesheet\" href=\"{0}\" {1}/>", helper.ContentStyleSheet(contentPath, includeVersion, area), resultClass));
        }
    }
}
