using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Sic.Web.Mvc
{
    public static class UrlContentExtensions
    {        
        public static string CombinePath(string contentPath1, string contentPath2)
        {
            return string.Format("{0}/{1}",contentPath1, contentPath2);
        }        
      
        public static string ContentScript(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion = false, string areaName = null)
        {
            string initialPath = CombinePath(Sic.Configuration.SicConfigurationSection.Current.MainContent.Path, areaName??"");
            initialPath = CombinePath(initialPath, Sic.Configuration.SicConfigurationSection.Current.Script.Path);

            return helper.Content(CombinePath(initialPath, contentPath), includeVersion);            
        }

        public static string ContentStyleSheet(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion = false, string areaName = null)
        {
            string initialPath = CombinePath(Sic.Configuration.SicConfigurationSection.Current.MainContent.Path, areaName??"");
            initialPath = CombinePath(initialPath, Sic.Configuration.SicConfigurationSection.Current.StyleSheet.Path);

            return helper.Content(CombinePath(initialPath, contentPath), includeVersion);
        }

        public static string ContentImage(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion = false, string areaName = null)
        {
            string initialPath = CombinePath(Sic.Configuration.SicConfigurationSection.Current.MainContent.Path, areaName??"");
            initialPath = CombinePath(initialPath, Sic.Configuration.SicConfigurationSection.Current.Image.Path);
            
            return helper.Content(CombinePath(initialPath, contentPath), includeVersion);
        }



        public static string Content(this System.Web.Mvc.UrlHelper helper, string contentPath, bool includeVersion)
        {
            if (!includeVersion)
                return helper.Content(contentPath);
            else
                return helper.ContentVersion(contentPath);
        }

        public static string ContentVersion(this System.Web.Mvc.UrlHelper helper, string contentPath)
        {
            string version = Sic.Configuration.SicConfigurationSection.Current.FileImportVersion;
            return string.Format("{0}?v={1}", helper.Content(contentPath), version);
        }
    }
}
