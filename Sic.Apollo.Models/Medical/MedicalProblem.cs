using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbMedicalProblem", Schema="med")]
    public class MedicalProblem: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MedicalProblemId { get; set; }

        public byte Type { get; set; }

        public MedicalProblemType MedicalProblemType
        {
            get
            {
                return (MedicalProblemType)this.Type;
            }
        }

        public string Name { get; set; }

        public short Priority { get; set; }

        public bool Active { get; set; }

        public virtual List<MedicalHistory> MedicalHistories { get; set; }
    }
}
