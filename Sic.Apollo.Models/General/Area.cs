using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.General
{
    [Table("tbArea",Schema="gen")]
    public class Area: EntityBase
    {
        [Key]
        public int AreaId { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}