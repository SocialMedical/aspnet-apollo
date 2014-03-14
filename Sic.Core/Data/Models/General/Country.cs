using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Models.General
{
    [Table("tbCountry", Schema="gen")]
    public class CountryBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }

        public string Name { get; set; }

        public string IsoCode { get; set; }

        public bool Active { get; set; }
    }
}
