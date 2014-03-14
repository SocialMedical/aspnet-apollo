using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbVademecum", Schema="med")]
    public class Vademecum: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long VademecumId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public virtual List<ProfessionalVademecum> ProfessionalVademecums { get; set; }        
    }
}