using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Appointment.View
{
    public class ProfessionalAppointment
    {
        public int ProfessionalId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTransactionId { get; set; }
        public int ProfessionalOfficeId { get; set; }        
        public int? CustomerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CustomerNameReference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReasonForVisit { get; set; }        
        public string InsuranceInstitution { get; set; }
        public string ContactPhoneNumber { get; set; }               
        public string CustomerNotes { get; set; }
        public byte State { get; set; }
        public bool IsCustomerAppointement { get; set; }

        public AppointmentState AppointmentState
        {
            get
            {
                AppointmentState result;
                if (((Sic.AppointmentState)this.State == Sic.AppointmentState.PendingConfirmation
                        || (Sic.AppointmentState)this.State == Sic.AppointmentState.PendingConfirmation) &&
                        this.StartDate < DateTime.Now)
                    {
                        result = Sic.AppointmentState.Obsolete;
                    }
                    else
                    {
                        result = (Sic.AppointmentState)this.State;
                    }
                return result;
            }
        }


        public int ContactLocationId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAddress")]
        public string OfficeAddress { get; set; }                

        public int Duration { get { return Convert.ToInt32((this.EndDate - this.StartDate).TotalMinutes); } }

        [StringLength(200)]
        public string AttentionNotes { get; set; }

        public string StateDisplay { get { return this.AppointmentState.GetDisplay(typeof(AppointmentState)); } }

        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return LastName + ", " + FirstName;
                else
                {
                    if (!string.IsNullOrEmpty(this.FirstName))
                        return FirstName;
                    else
                        return CustomerNameReference;
                }
            }
        }        
    }
}