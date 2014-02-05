using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Web.Mvc;

namespace Sic.Apollo.Models.Appointment.View
{
    public class AppointmentTransaction
    {
        public long AppointmentId { get; set; }

        public long AppointmentTransactionId { get; set; }   

        public int SpecializationId { get; set; }

        public int? CustomerId { get; set; }

        public int Step { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentToStringDateTimeFormat {
            get
            {
                return this.AppointmentDate.ToString("f");//"dddd, MMMM d, yyyy / HH:mm tt");//"Miercoles, 10 de octrubre 2012 / 08:30 am";
            }
        }

        public Pro.Customer Customer { get; set; }

        public int ProfessionalId { get; set; }

        public Pro.View.Professional Professional { get; set; }

        public int ContactLocationId { get; set; }

        public int? SpecializationAppointmentReasonId { get; set; }

        public Models.Appointment.SpecializationAppointmentReason SpecializationAppointmentReason { get; set; }

        public int? InsuranceInstitutionId { get; set; }

        public Pro.InsuranceInstitution InsuranceInstitution { get; set; }

        [RequiredIf("UseInsurance", true, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        public bool UseInsurance { get; set; }

        [StringLength(50)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        public string ContactPhoneNumber { get; set; }

        public bool SendMeReminder { get; set; }

        [StringLength(200)]
        public string CustomerNotes { get; set; }
    }
}