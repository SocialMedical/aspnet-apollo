using Sic.Apollo.Models.Pro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sic.Apollo.Models.Appointment.View
{
    public class AppointmentParameters
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Duration { get; set; }

        public int ContactLocationId { get; set; }

        public int AppointmentTransactionId { get; set; }

        public IEnumerable<DayOfWeek> DaysOfWeek { get; set; }

        public IEnumerable<ProfessionalOfficeSchedule> Schedules { get; set; }
    }
}
