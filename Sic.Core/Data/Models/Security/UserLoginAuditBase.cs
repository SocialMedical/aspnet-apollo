using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Data.Models.Security
{
    [Table("tbUserLoginAudit", Schema = "sec")]
    public class UserLoginAuditBase: Sic.Data.Entity.EntityBase
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserLoginAuditId { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public DateTime LoginDate { get; set; }

        public virtual UserBase User { get; set; }

        public virtual RoleBase Role { get; set; }
    }
}
