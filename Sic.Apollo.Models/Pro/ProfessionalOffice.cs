using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalOffice", Schema = "pro")]   
    public class ProfessionalOffice: ContactLocation
    {        		        
        public int ProfessionalId { get; set; }		

		public bool Active { get; set; }

        public override int Key
        {
            get
            {
                return base.ContactLocationId;
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Address;
            }
        }

        public virtual Professional Professional { get; set; }

        public virtual List<ProfessionalOfficeSchedule> ProfessionalOfficeSchedules { get; set; }
    }
}