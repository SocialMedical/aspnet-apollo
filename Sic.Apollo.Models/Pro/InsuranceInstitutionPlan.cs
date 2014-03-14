using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Medical;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbInsuranceInstitutionPlan", Schema = "pro")]
    public class InsuranceInstitutionPlan : EntityBase
    {
       [Key]
       public int InsuranceInstitutionPlanId { get; set; }

	   public int InstitutionId { get; set; }

	   public string Name { get; set; }

       public bool Active { get; set; }
       
       public virtual InsuranceInstitution InsuranceInstitution { get; set; }

       public virtual List<PatientInsuranceInstitution> PatientInsuranceInstitutions { get; set; }

       public override string DescriptionName
       {
           get
           {
               return this.Name;
           }
       }
    }
}