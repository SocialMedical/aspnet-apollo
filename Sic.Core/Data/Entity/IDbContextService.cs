using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.Entity
{
    public interface IDbContextService
    {
        void Save();
        void Dispose();
    }
}
