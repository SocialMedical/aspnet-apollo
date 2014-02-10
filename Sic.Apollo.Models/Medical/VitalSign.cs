using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbVitalSign", Schema="med")]
    public class VitalSign: Sic.Data.Entity.EntityBase
    {
        [Key]
        public int VitalSignId { get; set; }
        
        public string Name { get; set; }

	    public short Priority { get; set; }

        public short DefaultMeasurementUnit { get; set; }

	    public bool Active { get; set; }

        public string Code { get; set; }

        public virtual List<PatientVitalSign> PatientVitalSigns { get; set; }
    }
}
