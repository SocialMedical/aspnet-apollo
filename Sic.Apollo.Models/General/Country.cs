using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.General
{
    [Table("tbCountry", Schema = "gen")]
    public class Country : EntityBase
    {
        [Key]
        public int CountryId { get; set; }
        
        public string Name { get; set; }

        public string Flag { get; set; }

        public string DefaultCulture { get; set; }
    }
}