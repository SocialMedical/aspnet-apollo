using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sic.Runtime
{
    public class Current
    {
        public static int UserId
        {
            get
            {
                if (HttpContext.Current.Session != null)
                    return Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                else
                    return 0;
            }
        }

        public static DateTime GetCurrentDateTime()
        {
            TimeZoneInfo.GetSystemTimeZones();
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), nzTimeZone);
            //return DateTime.Now.ToUniversalTime();
        }
    }
}
