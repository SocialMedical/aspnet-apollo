using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Data.Models.Security
{
    [Table("tbRole", Schema="sec")]
    public class RoleBase: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public bool IsDefault { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual OrganizationBase Organization { get; set; }

        public virtual List<UserRoleBase> UserRoles { get; set; }

        [ForeignKey("RoleId")]
        public virtual List<UserLoginAuditBase> UserLoginAudits { get; set; }
    }
}
