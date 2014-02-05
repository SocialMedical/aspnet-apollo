using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbMedicalCare", Schema="med")]
    public class MedicalCare: Sic.Data.Entity.EntityBase
    {
        [Key]
        public long MedicalCareId { get; set; }

	    public int PatientId { get; set; } 

	    public int ProfessionalId { get; set;}

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
	    public DateTime RecordDate { get; set;}
        
        public DateTime? UpdateRecordDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        public DateTime EvolutionDate { get; set; }

        [NotMapped]
        public long EvolutionDateTicks
        {
            get
            {
                return this.EvolutionDate.Ticks;
            }
            set
            {
                this.EvolutionDate = new DateTime(value);
            }
        }
        
        public bool IsEmptyHeader {             
            get    {
                return string.IsNullOrWhiteSpace(this.Evolution)
                    && string.IsNullOrWhiteSpace(this.Diagnostic)
                    && string.IsNullOrWhiteSpace(this.Treatment);
                }
        }

	    public string Evolution { get; set; }
        
        public string Diagnostic { get; set; }

        public string Treatment { get; set; }

        [NotMapped]
        public string MedicationsClientXml { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual List<MedicalCareMedication> MedicalCareMedications { get; set; }
    }
}
