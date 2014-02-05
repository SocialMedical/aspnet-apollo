using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.Entity
{
    public class AuditableEntityBase: Sic.Data.Entity.EntityBase, IAuditable
    {
        public int UserInsertId { get; set; }

        public DateTime DateInsert { get; set; }

        public int? UserUpdateId { get; set; }

        public DateTime? DateUpdate { get; set; }  
    }
}
