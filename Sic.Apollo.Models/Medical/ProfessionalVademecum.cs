using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbProfessionalVademecum",Schema="med")]
    public class ProfessionalVademecum: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ProfessionalVademecumId { get; set; }

        public int ProfessionalId { get; set; }

        public long? VademecumId { get; set; }

        public string Name { get; set; }

        public string Posology { get; set; }

        public bool Active { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual Vademecum Vademecum { get; set; }

        public virtual List<MedicalCareMedication> MedicalCareMedications { get; set; }
    }
}
