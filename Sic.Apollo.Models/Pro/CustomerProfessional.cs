using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbCustomerProfessional", Schema = "pro")]
    public class CustomerProfessional : EntityBase
    {
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int CustomerProfessionalId { get; set; }

        public int CustomerId { get; set; }
        public int ProfessionalId { get; set; }
    }
}
