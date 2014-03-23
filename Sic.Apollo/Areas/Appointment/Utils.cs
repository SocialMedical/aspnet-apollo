using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Areas.Appointment
{
    public class Utils
    {
        public static IEnumerable<Apollo.Models.Appointment.View.DayOfWeek> GetDaysOfWeek(DateTime? startDate = null, bool fixedCurrentDate = true)
        {
            DateTime date = Sic.Web.Mvc.Session.CurrentDateTime;
            if (startDate != null && (fixedCurrentDate || startDate.Value > date))
                date = startDate.Value;

            List<Apollo.Models.Appointment.View.DayOfWeek> wa = new List<Apollo.Models.Appointment.View.DayOfWeek>();
            for (int i = 1; i <= 7; i++)
            {
                DateTime dateStart = date.Date;
                if (dateStart.Date == Sic.Web.Mvc.Session.CurrentDateTime.Date)
                    dateStart = Sic.Web.Mvc.Session.CurrentDateTime;

                wa.Add(new Apollo.Models.Appointment.View.DayOfWeek(dateStart));
                date = date.AddDays(1);
            }

            return wa.OrderBy(p => p.Date);
        }

        public static IEnumerable<Apollo.Models.Appointment.View.DayOfWeek> DayOfWeek(long? startTime = null)
        {
            DateTime? date = null;
            if (startTime != null)
                date = new DateTime(startTime.Value);

            return GetDaysOfWeek(date);
        }
    }
}