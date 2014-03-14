using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalOffice", Schema = "pro")]   
    public class ProfessionalOffice: ContactLocation
    {        		        
        public int ProfessionalId { get; set; }		

		public bool Active { get; set; }

        public override string Key
        {
            get
            {
                return base.ContactLocationId.ToString();
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