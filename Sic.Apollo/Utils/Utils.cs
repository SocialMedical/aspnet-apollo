using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace Sic.Apollo
{
    public static class Utils
    {
        public static string DefaultProfessionalPicture
        {
            get
            {
                return "/Content/img/contacts/DefaultProfessional.jpg";
            }
        }
        public static string ToDefaultDateTimeFormat(this DateTime dateTime)
        {
            return string.Format(Sic.Apollo.Resources.Resources.StringFormatForDefaultDateTime, dateTime);
        }

        public static string ToDefaultDateFormat(this DateTime dateTime)
        {
            return string.Format(Sic.Apollo.Resources.Resources.StringFormatForDefaultDate, dateTime);
        }

        public static string FileToString(string filePath)
        {
            StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath(filePath));
            string template = streamReader.ReadToEnd();
            streamReader.Close();

            return template;
        }
       
    }
}