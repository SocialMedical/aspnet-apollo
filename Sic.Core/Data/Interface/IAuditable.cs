using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.Entity
{
    interface IAuditable
    {
        int UserInsertId { get; set; }

        DateTime DateInsert { get; set; }

        int? UserUpdateId { get; set; }

        DateTime? DateUpdate { get; set; }  
    }
}
