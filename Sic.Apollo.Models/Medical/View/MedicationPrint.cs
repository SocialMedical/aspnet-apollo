using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.Medical.View
{
    public class MedicationPrint
    {
        public Professional Professional { get; set; }

        public Models.Medical.Patient Patient { get; set; }      

        public List<MedicalCareMedication> MedicalCareMedications { get; set; }
    }
}
