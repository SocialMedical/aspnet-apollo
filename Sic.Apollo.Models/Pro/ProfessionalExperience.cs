using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalExperience", Schema = "pro")]
    public class ProfessionalExperience: EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ProfessionalExperienceId { get; set; }

        public int ProfessionalId { get; set; }

        public int? InstitutionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForProfessionalExperience")]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDetail")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForStartYear")]
        public int? StartYear { get; set; }

        [CompareValues("StartYear", CompareValues.GreatThanOrEqualTo, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldComparer")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForEndYear")]
        public int? EndYear { get; set; }

        public virtual Institution Institution { get; set; }
    }
}