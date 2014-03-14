using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Models.General;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Models.Security
{
    [Table("tbUser", Schema="sec")]
    public class UserBase : Sic.Data.Entity.EntityBase
    {
        [Key, ForeignKey("Contact")]
        public int UserId { get; set; }

        public string LogonName { get; set; }        

        public string Password { get; set; }

        [NotMapped]
        [Description("Only for Reset Password Proposity")]
        public string DecodePassword { get; set; }

        public bool Active { get; set; }

        public virtual List<UserRoleBase> UserRole { get; set; }
       
        public virtual ContactBase Contact { get; set; }

        [ForeignKey("UserId")]
        public virtual List<UserLoginAuditBase> UserLoginAudits { get; set; }
    }
}
