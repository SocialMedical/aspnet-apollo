using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using Sic.Apollo.Models.Medical;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbInsuranceInstitution", Schema = "pro")]
    public class InsuranceInstitution : Institution
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForActive")]
        public bool Active { get; set; }

        public override string Key
        {
            get
            {
                return this.InstitutionId.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Contact.FirstName;
            }
        }

        public short Priority { get; set; }

        public virtual List<InsuranceInstitutionPlan> InsuranceInstitutionPlans { get; set; }
        
        public virtual List<Appointment.AppointmentTransaction> AppointmentTransactions { get; set; }

        public virtual List<PatientInsuranceInstitution> PatientInsuranceInstitutions { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();

            this.Contact = new General.Contact();
            this.Contact.Gender = (int)Gender.NotApplicable;            
            this.Type = (int)InstitutionType.Insurance;
            this.Active = true;
        }
    }
}