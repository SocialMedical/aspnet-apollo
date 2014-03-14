using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Models.Security
{
    [Table("tbUserRole",Schema="sec")]
    public class UserRoleBase: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }

        public int UserId { get; set; }        

        public int RoleId { get; set; }        

        [ForeignKey("UserId")]
        public virtual UserBase User { get; set; }

        [ForeignKey("RoleId")]
        public virtual RoleBase Role { get; set; }        
    }
}
