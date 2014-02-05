using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Data.Models.Security
{
    [Table("tbOrganization", Schema = "sec")]
    public class OrganizationBase : Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public virtual int OrganizationId { get; set; }

        public virtual string Name { get; set; }

        public virtual string SubDomain { get; set; }        

        public virtual int State { get; set; }
    }    
}