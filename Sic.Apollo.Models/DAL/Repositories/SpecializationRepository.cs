using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Apollo.Models.Pro;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Repositories
{
    public class SpecializationRepository : Sic.Data.Entity.Repository<Specialization>
    {
        public SpecializationRepository(DbContext context)
            :base(context)
        {
        }

        public Specialization GetByName(string name)
        {
            Context db = (Context)context;
            return db.Specializations.Single(p => p.Name == name);
        }
    }
}
