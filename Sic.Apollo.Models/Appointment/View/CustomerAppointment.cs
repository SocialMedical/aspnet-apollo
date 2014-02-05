using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Appointment.View
{
    public class CustomerAppointment
    {
        public long AppointmentId { get; set; }
        public long AppointmentTransactionId { get; set; }
        public int SpecializationId { get; set; }
        public int? SpecializationAppointmentReasonId { get; set; }
        public int ProfessionalId { get; set; }
        public int ContactLocationId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime StartDate { get; set; }
        public string ReasonForVisit { get; set; }
        public byte State { get; set; }        
        public string InsuranceInstitution { get; set; }
        public string Picture { get; set; }

        public int? RateScore1 { get; set; }
        public int? RateScore2 { get; set; }
        public int? RateScore3 { get; set; }
        [StringLength(200)]
        public string RateComments { get; set; }

        public string StateDisplay { get { return this.State.GetDisplay(typeof(AppointmentState)); } }

        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return LastName + ", " + FirstName;
                else
                    return FirstName;
            }
        }
    }
}