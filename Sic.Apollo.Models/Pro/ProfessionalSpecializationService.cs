using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("ProfessionalSpecializationService", Schema="pro")]
    public class ProfessionalSpecializationService: EntityBase
    {
        [Key]
        public int ProfessionalSpecializationServiceId { get; set; }

	    public int ProfessionalSpecializationId { get; set; }

	    public int SpecializationServiceId { get; set; }

	    public string Name { get; set; }

	    public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForActive")] 
        public bool Active { get; set; }

        public virtual SpecializationService SpecializationService { get; set; }        
    }
}