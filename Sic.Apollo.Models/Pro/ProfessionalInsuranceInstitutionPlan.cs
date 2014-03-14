using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalInsuranceInstitutionPlan", Schema = "pro")]
    public class ProfessionalInsuranceInstitutionPlan: EntityBase
    {
       [Key]
       public int ProfessionalInsuranceInstitutionPlanId { get; set;}

       public int InstitutionId { get; set; }

	   public int ProfessionalId { get; set;}

	   public int InsuranceInstitutionPlanId { get; set;}

       public bool Active { get; set; }

       public virtual InsuranceInstitutionPlan InsuranceInstitutionPlan { get; set; }
       
       public virtual InsuranceInstitution InsuranceInstitution { get; set; }

       public virtual Professional Professional { get; set; }
       
    }
}