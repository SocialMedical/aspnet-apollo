using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.General
{
    [Table("tbCity", Schema = "gen")]
    public class City : EntityBase
    {
        [Key]
        public int CityId { get; set; }

        public string Name { get; set; }

        public int StateId { get; set; }

        public State State { get; set; }

        public short Priority { get; set; }

        public bool IsDefault { get; set; }

        public virtual List<ContactLocation> ContactLocations { get; set; } 

        public override string Key
        {
            get
            {
                return this.CityId.ToString();
            }           
        }

        public override string DescriptionName
        {
            get
            {
                return this.Name;
            }           
        }
    }
}