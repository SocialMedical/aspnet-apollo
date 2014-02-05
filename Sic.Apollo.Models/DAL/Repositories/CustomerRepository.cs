using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Apollo.Models.Pro;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Repositories
{
    public class CustomerRepository : Sic.Data.Entity.Repository<Customer>
    {
        public CustomerRepository(DbContext context)
            :base(context)
        {
        }

        public override Customer GetByID(object id)
        {
            int key = Convert.ToInt32(id);
            return Get(p => p.CustomerId == key, null, "Contact").SingleOrDefault();
        }

        public List<Pro.View.Professional> GetCustomerProfessionals(int CustomerId)
        {
            Context db = (Context)context;

            return (from p in db.Professionals
                    join u in db.CustomerProfessionals on p.ProfessionalId equals u.ProfessionalId
                    where u.CustomerId == CustomerId
                    select new Pro.View.Professional
                    {
                        ProfessionalId = p.ProfessionalId,
                        Picture = p.Contact.Picture,
                        FirstName = p.Contact.FirstName,
                        MiddleName = p.Contact.MiddleName,
                        LastName = p.Contact.LastName,
                        SecondLastName = p.Contact.SecondLastName,
                        RateScore = p.RateScore                        
                    }).ToList();
        }
    }
}