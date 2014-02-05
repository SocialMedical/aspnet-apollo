using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Appointment.View
{
    public class AppointmentBook
    {
        public long AppointmentId { get; set; }

        public int ProfessionalId { get; set; }
        
        public int? SpecializationAppointmentReasonId { get; set; }

        public int? InsuranceInstitutionId { get; set; }
    }
}