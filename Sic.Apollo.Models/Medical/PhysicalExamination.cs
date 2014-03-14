using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPhysicalExamination", Schema="med")]
    public class PhysicalExamination: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int PhysicalExaminationId { get; set; }

        public string Name { get; set; }

        public short Priority { get; set; }

        public bool Active { get; set; }        
    }
}
