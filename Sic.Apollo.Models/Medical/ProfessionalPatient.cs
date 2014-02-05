using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbProfessionalPatient", Schema="med")]
    public class ProfessionalPatient: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public long ProfessionalPatientId { get; set; }
                    
        public int ProfessionalId { get; set; }
        
        public int PatientId { get; set; }

        public DateTime? LastChange { get; set; }

        public DateTime? NextAppointmentDate { get; set; }

        public byte State { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Professional Professional { get; set; }
    }
}
