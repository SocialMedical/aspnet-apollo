using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Models.Appointment.View
{
    public class ProfessionalOfficeAppointment
    {
        public int ProfessionalOfficeId { get; set; }
        public string Description { get; set; }

        public List<ProfessionalAppointment> ProfessionalAppointments { get; set; }  
    }
}