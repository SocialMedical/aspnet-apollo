using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models.Medical.View
{
    public class Vademecum
    {
        public long ProfessionalVademecumId { get; set; }

        public long? VademecumId { get; set; }

        public string Name { get; set; }

        public string Posology { get; set; }
    }
}
