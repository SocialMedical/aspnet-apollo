using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.General
{
    [Table("tbState", Schema = "gen")]
    public class State : EntityBase
    {
        [Key]
        public int StateId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }
    }
}