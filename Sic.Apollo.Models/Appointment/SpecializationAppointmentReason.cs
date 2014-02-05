﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.Appointment
{
    [Table("tbSpecializationAppointmentReason", Schema = "apo")]
    public class SpecializationAppointmentReason: EntityBase
    {
        [Key]
        public int SpecializationAppointmentReasonId { get; set; }

        public int SpecializationId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public Specialization Specialization { get; set; }

        public override int Key
        {
            get
            {
                return this.SpecializationAppointmentReasonId;
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