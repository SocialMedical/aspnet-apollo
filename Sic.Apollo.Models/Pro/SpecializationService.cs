﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbSpecializationService", Schema = "pro")]
    public class SpecializationService: EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SpecializationServiceId { get; set; }
        
        public int SpecializationId { get; set; }

	    public string Name { get; set; }
        
        public string Description { get; set; }
    }
}