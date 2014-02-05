using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sic.Web.Mvc.Utility
{
    public class Thumbnail
    {
        public static string GetPictureMinFromOriginal(string fileName)
        {
            return Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_min" + Path.GetExtension(fileName);
        }

        public static string GetPictureMediumFromOriginal(string fileName)
        {
            return Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_med" + Path.GetExtension(fileName);
        }
    }
}
