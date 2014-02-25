using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sic.Web.Mvc.Utility
{
    public class Thumbnail    
    {
        public static string SaveThumbnail(string fileName, int? width = null, int? height = null, string lit = null)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);

            if (!width.HasValue)
            {
                width = 70;
                height = (width * img.Height) / img.Width;
                //double maxHeight = 70;                

                //if (img.Height / maxHeight >= img.Width / maxWidth)
                //{
                //    factor = maxHeight * 1.0 / img.Height;
                //}
                //else
                //{
                //    factor = maxWidth * 1.0 / img.Width;
                //}

                //height = (int)Math.Round(img.Height * factor);
                //width = (int)Math.Round(img.Width * factor);
            }

            if (string.IsNullOrEmpty(lit))
                lit = "min";

            System.Drawing.Image mini = img.GetThumbnailImage(width.Value, height.Value, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

            string minFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "_" + lit + Path.GetExtension(fileName));

            mini.Save(minFileName);

            return minFileName;
        }

        private static bool ThumbnailCallback()
        {
            return false;
        }

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
