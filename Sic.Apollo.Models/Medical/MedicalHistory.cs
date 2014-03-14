using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbMedicalHistory", Schema="med")]
    public class MedicalHistory: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long MedicalHistoryId { get; set; }

        public int ProfessionalId { get; set; }

        public int PatientId { get; set; }

        public int MedicalProblemId { get; set; }

        public DateTime RecordDate { get; set; }

        public string Description { get; set; }

        public byte Frecuency { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual MedicalProblem MedicalProblem { get; set; }
    }
}
