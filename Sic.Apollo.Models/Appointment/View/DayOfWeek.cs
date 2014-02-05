using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace Sic.Apollo.Models.Appointment.View
{
    public class DayOfWeek
    {
        public DayOfWeek(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; set; }
        public string DayName
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames[(int)Date.DayOfWeek];
            }
        }
    }
}