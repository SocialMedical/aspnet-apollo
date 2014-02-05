using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbMedicalCareMedication", Schema="med")]
    public class MedicalCareMedication: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public long MedicalCareMedicationId { get; set; }

        public long MedicalCareId { get; set; }

        public long? ProfessionalVademecumId { get; set; }

        public string MedicationName { get; set; }

        public string Posology { get; set; }

        public short Quantity { get; set; }

        public short Priority { get; set; }

        public MedicalCare MedicalCare { get; set; }

        public ProfessionalVademecum ProfessionalVademecum { get; set; }
    }
}
