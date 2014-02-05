using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Models.Appointment.View
{
    public class ContactLocationAppointments
    {
        public int ContactLocationId { get; set; }

        public int SpecializationId { get; set; }

        public List<AppointmentEntry> AppointmentEntries { get; set; }        
    }
}