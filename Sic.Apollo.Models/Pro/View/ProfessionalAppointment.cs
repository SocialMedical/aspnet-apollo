using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Appointment.View;

namespace Sic.Apollo.Models.Pro.View
{
    public class ProfessionalAppointment: View.Professional
    {        
        public List<AppointmentEntry> AppointmentEntries { get; set; } 
    }   
}