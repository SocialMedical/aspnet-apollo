using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Models.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Models.General
{
    [Table("tbContact",Schema="gen")]
    public class ContactBase: Sic.Data.Entity.EntityBase, INameable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ContactId { get; set; }

        public string Names { get; set; }

        public string LastNames { get; set; }

        public string RegisterOrganizationName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string DefaultFullName
        {
            get
            {
                return Sic.Data.Service.GetDefaultFullName(this);
            }
        }

        public virtual List<ContactUs> ContactUs { get; set; }

        public DateTime? RegisterDate { get; set; }

        public virtual UserBase User { get; set; }
    }
}
