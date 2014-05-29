using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sic.Apollo.Models.Medical.View
{
    [NotMapped]
    public class MedicalCareEdit : Models.Medical.MedicalCare
    {
        public List<MedicalCareMedicationEdit> Medications { get; set; }
    }

    [NotMapped]
    public class MedicalCareMedicationEdit : Models.Medical.MedicalCareMedication
    {
        public long GeneralVademecumId { get; set; }
    }
}
