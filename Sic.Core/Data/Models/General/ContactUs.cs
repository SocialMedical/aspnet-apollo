using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Models.General
{
    [Table("tbContactUs", Schema = "gen")]
    public class ContactUs: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactUsId { get; set; }

        public int ContactId { get; set; }

        public DateTime RecordDate { get; set; }

        public string Comments { get; set; }

        public virtual ContactBase Contact { get; set; }
    }
}
