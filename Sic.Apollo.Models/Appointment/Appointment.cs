using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Appointment
{
    [Table("tbAppointment", Schema = "apo")]
    public class Appointment: EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long AppointmentId { get; set; }
                 
	    public int ProfessionalId { get; set; }
        
	    public int ContactLocationId { get; set; }

        public int? ProfessionalOfficeScheduleId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentStartDate")]
	    public DateTime StartDate { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentEndDate")]
	    public DateTime EndDate { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentState")]
        public byte State { get; set; }

        public string StateDisplay { get { return this.State.GetDisplay(typeof(AppointmentState)); } }

        public virtual Professional Professional { get; set; }

        public virtual ProfessionalOffice ProfessionalOffice { get; set; }

        public virtual ProfessionalOfficeSchedule ProfessionalOfficeSchedule { get; set; }

        public virtual List<AppointmentTransaction> AppointmentTransactions { get; set; }
    }
}