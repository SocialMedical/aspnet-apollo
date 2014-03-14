using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbInstitution", Schema = "pro")]
    public class Institution : EntityBase
    {        
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int InstitutionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForType")]     
        public int Type { get; set; }

        public string TypeDisplay { get { return this.Type.GetDisplay(typeof(InstitutionType)); } }

        [ForeignKey("InstitutionId")]
        public virtual Contact Contact { get; set; }

        public virtual List<ProfessionalSchool> ProfessionalSchools { get; set; }

        public virtual List<ProfessionalExperience> ProfessionalExperiences { get; set; }

        public virtual List<ProfessionalCommunity> ProfessionalCommunities { get; set; } 
    }
}