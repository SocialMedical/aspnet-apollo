using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.Repositories
{
    public class InsuranceInstitutionRepository : Sic.Data.Entity.Repository<InsuranceInstitution>
    {
        public InsuranceInstitutionRepository(DbContext context):base(context)
        {
        }

        public override IEnumerable<InsuranceInstitution> Get(System.Linq.Expressions.Expression<Func<InsuranceInstitution, bool>> filter = null, Func<IQueryable<InsuranceInstitution>, IOrderedQueryable<InsuranceInstitution>> orderBy = null, string includeProperties = "")
        {
            if (string.IsNullOrEmpty(includeProperties))
                includeProperties = "Contact";

            return base.Get(filter, orderBy, includeProperties);
        }

        public override InsuranceInstitution GetByID(object id)
        {
            int key = Convert.ToInt32(id);
            return Get(p => p.InstitutionId == key, null, "Contact").SingleOrDefault();
        }       
    }
}