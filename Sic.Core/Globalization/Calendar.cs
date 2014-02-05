using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Sic.Core.Globalization
{
    public class Calendar
    {
        public static int DiffWeeks(DateTime start, DateTime end, DayOfWeek firstDayOfWeek)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            System.Globalization.Calendar calendar = dfi.Calendar;
            int weekNumber1 = calendar.GetWeekOfYear(start, dfi.CalendarWeekRule, firstDayOfWeek);
            int weekNumber2 = calendar.GetWeekOfYear(end, dfi.CalendarWeekRule, firstDayOfWeek);
            int compareValue1 = (start.Year * 100) + weekNumber1;
            int compareValue2 = (end.Year * 100) + weekNumber2;
            return compareValue2 - compareValue1;
        }
    }
}
