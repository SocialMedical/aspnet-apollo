using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Security;
using Sic.Web.Mvc;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbProfessionalTeam",Schema="pro")]
    public class ProfessionalTeam: Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ProfessionalTeamId { get; set; }

        public int ProfessionalId { get; set; }

        public int TeamUserId { get; set; }

        public bool Active { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual User TeamUser { get; set; }
    }
}