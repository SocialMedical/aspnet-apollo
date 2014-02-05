﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Web.Mvc;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalCommunity", Schema = "pro")]
    public class ProfessionalCommunity: EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int ProfessionalCommunityId { get; set; }

        public int ProfessionalId { get; set; }

        public int? InstitutionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForInstitution")]
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