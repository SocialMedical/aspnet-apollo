using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sic.Data.Models.General
{
    [Table("tbContactLocation", Schema="gen")]
    public class ContactLocationBase: Sic.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return this.ContactLocationId.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Description;
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactLocationId { get; set; }

        public int? ContactId { get; set;}

        public string Description { get; set; }

	    public string Address { get; set;}

	    public string AddressLine2 { get; set;}

        public int CountryId { get; set; }

	    public string CityName { get; set;}

	    public string StateName { get; set;}

	    public string DefaultPhoneNumber { get; set;}

	    public string DefaultPhoneExtension { get; set;}

	    public string PhoneNumber01 { get; set;}

	    public string PhoneExtension01 { get; set;}

	    public string Email { get; set;}

        public string WebSite { get; set; }

        public ContactBase Contact { get; set; }
        
        public CountryBase Country { get; set; }
    }
}
