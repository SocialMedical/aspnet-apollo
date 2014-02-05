using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPatientVitalSign",Schema="med")]
    public class PatientVitalSign: Sic.Data.Entity.EntityBase
    {
        [Key]
        public long PatientVitalSignId { get; set; }

	    public int ProfessionalId { get; set; }

	    public int PatientId { get; set; }

	    public DateTime RecordDate { get; set; }

        public DateTime VitalSignDate { get; set; }

	    public int VitalSignId { get; set; }

	    public string Value { get; set; }

        public short MeasuringUnit { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual VitalSign VitalSign { get; set; }

        public virtual Professional Professional { get; set; }
    }
}
