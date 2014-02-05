using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalSpecialization", Schema = "pro")]
    public class ProfessionalSpecialization : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int ProfessionalSpecializationId { get; set; }
        
        public int ProfessionalId { get; set; }

        public int SpecializationId { get; set; }

        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }

        [ForeignKey("SpecializationId")]
        public virtual Specialization Specialization { get; set; }

        public virtual List<ProfessionalSpecializationService> ProfessionalSpecializationServices { get; set; }
    }
}