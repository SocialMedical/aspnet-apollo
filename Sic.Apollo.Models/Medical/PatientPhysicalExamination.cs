using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPatientPhysicalExamination", Schema = "med")]
    public class PatientPhysicalExamination: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long PatientPhysicalExaminationId { get; set; }

        public int ProfessionalId { get; set; }

        public int PatientId { get; set; }

        public DateTime RecordDate { get; set; }

        public int PhysicalExaminationId { get; set; }

        public string Examination { get; set; }

        public virtual PhysicalExamination PhysicalExamination { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
