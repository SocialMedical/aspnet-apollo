using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Models.Pro.View
{
    [Serializable]
    public class ProfessionalOffice
    {
        public int ContactLocationId { get; set; }

        public int ProfessionalId { get; set; }

        public string Description { get; set; }        

        public string Address { get; set; }
        
        public string IntersectionStreet { get; set; }

        public int NumberInStreet { get; set; }

        public string References { get; set; }

        public string DefaultPhoneNumber { get; set; }

        public string DefaultPhoneExtension { get; set; }

        public string PhoneNumber01 { get; set; }

        public string PhoneExtension01 { get; set; }   

        public int CityId { get; set; }
    }
}