using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbSpecialization", Schema = "pro")]    
    public class Specialization : EntityBase
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int SpecializationId { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForName")]
        public string Name { get; set; }

        [StringLength(100)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForProfession")]
        public string Profession { get; set; }

        [StringLength(100)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForProfessionHelp")]
        public string Help { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        public bool IsDefault { get; set; }

        public string ProfessionInPlural { get; set; }

        public short Priority { get; set; }        

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForActive")]        
        public bool Active { get; set; }

        public virtual List<ProfessionalSpecialization> ProfessionalSpecializations { get; set; }

        public virtual List<SpecializationService> SpecializationServices { get; set; }

        public override string Key
        {
            get
            {
                return this.SpecializationId.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return string.Format("{0}{1}",this.Profession,string.IsNullOrEmpty(this.Help)?"":" (" + this.Help + ")");
            }
        }        

        public override void OnCreate()
        {
            base.OnCreate();

            this.Active = true;
            this.IsDefault = false;
        }
    }
}