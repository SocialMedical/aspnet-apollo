using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo.Models.Pro.View
{
    public class ProfessionalOfficeMap
    {
        public int ContactLocationId { get; set; }
        public int ProfessionalId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Street { get; set; }
        public int NumberInStreet { get; set; }
        public string IntersectionStreet { get; set; }
        public string Picture { get; set; }

        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return LastName + ", " + FirstName;
                else
                    return FirstName;
            }
        }

        public string Address
        {
            get
            {
                return String.Format("{0} {1} {2}", this.Street, this.NumberInStreet, this.IntersectionStreet);
            }
        }                
    }
}