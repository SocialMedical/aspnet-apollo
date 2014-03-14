using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using Sic.Data.Entity;
using Sic.Apollo.Models.Medical;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Appointment
{
    [Table("tbAppointmentTransaction", Schema = "apo")]
    public class AppointmentTransaction : EntityBase
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long AppointmentTransactionId { get; set; }
        
        public long AppointmentId { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerNameReference { get; set; }

        public int? ProfessionalCustomerScoreId { get; set; }                

        public DateTime TransactionDate { get; set; }

        public DateTime? CancelTransactionDate { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentUseInsurance")]
        public bool UseInsurance { get; set; }

        public int? InsuranceInstitutionId { get; set; }

        public int? InsuranceInstitutionPlanId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentState")]
        public byte State { get; set; }

        public string StateDisplay { get { return this.State.GetDisplay(typeof(AppointmentState)); } }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFirstAppointmentWithProfessional")]
        public bool FirstAppointmentWithThisProfessional { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentReason")]
        public int? SpecializationAppointmentReasonId { get; set; }

        public bool AppointmentForMe { get; set; }

        [StringLength(50)]
        public string ContactPhoneNumber { get; set; }

        public bool SendMeReminder { get; set; }

        [StringLength(200)]
        public string CustomerNotes { get; set; }

        [StringLength(200)]
        public string ConfirmToAttentionNotes { get; set; }

        [StringLength(200)]
        public string AttentionNotes { get; set; }

        public DateTime? RateDate { get; set; }

        public int? RateScore1 { get; set; }

        public int? RateScore2 { get; set; }

        public int? RateScore3 { get; set; }

        [StringLength(200)]
        public string RateComments { get; set; }
        
        public virtual Appointment Appointment { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual InsuranceInstitution InsuranceInstitution { get; set; }

        public virtual InsuranceInstitutionPlan InsuranceInstitutionPlan { get; set; }

        public virtual SpecializationAppointmentReason SpecializationAppointmentReason { get; set; }        
    }
}