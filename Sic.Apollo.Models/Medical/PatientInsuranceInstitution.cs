using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPatientInsuranceInstitution", Schema="med")]
    public class PatientInsuranceInstitution: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int PatientInsuranceInstitutionId { get; set; }

        public int PatientId { get; set; }

        public int? InsuranceInstitutionPlanId { get; set; }

        public string InsuranceInstitutionPlanName { get; set; }

        public int InsuranceInstitutionId { get; set; }

        public byte Priority { get; set; } 

        public virtual Patient Patient { get; set; }

        public virtual InsuranceInstitutionPlan InsuranceInstitutionPlan { get; set; }

        public virtual InsuranceInstitution InsuranceInstitution { get; set; }

        public string RegistrationCode { get; set; }
    }
}
