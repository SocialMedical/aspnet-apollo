using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models.Pro.View
{
    public class AssignedInsuranceInstitutionPlan
    {
        public AssignedInsuranceInstitutionPlan()
        {
            AssignedInsuranceInstitutionPlans = new List<AssignedInsuranceInstitutionPlanItem>();
        }
        public Models.Pro.Professional Professional { get; set; }
        public List<AssignedInsuranceInstitutionPlanItem> AssignedInsuranceInstitutionPlans { get; set; }
    }

    public class AssignedInsuranceInstitutionPlanItem
    {        
        public InsuranceInstitutionPlan InsuranceInstitutionPlan { get; set; }
        public bool Assigned { get; set; }
    }
}
