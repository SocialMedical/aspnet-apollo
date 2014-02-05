using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;

namespace Sic.File
{
    public class Utils
    {
        public static string FileToString(string filePath)
        {
            StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath(filePath));
            string template = streamReader.ReadToEnd();
            streamReader.Close();

            return template;
        }
    }
}
