using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Models.Appointment.View
{
    public class AppointmentEntry
    {
        public long AppointmentId { get; set; }
        public DateTime StartDate { get; set; }
        public int ContactLocationId { get; set; }
    }
}