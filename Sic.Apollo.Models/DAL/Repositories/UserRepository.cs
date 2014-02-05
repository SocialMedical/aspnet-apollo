using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using Sic.Apollo.Models.Security;

namespace Sic.Apollo.Models.Repositories
{
    public class UserRepository : Sic.Data.Entity.Repository<User>
    {
        public UserRepository(DbContext context)
            :base(context)
        {
        }

        public override User GetByID(object id)
        {
            int key = Convert.ToInt32(id);
            return Get(p => p.UserId == key, null, "Contact").SingleOrDefault();
        }

        public User GetByLogonName(string logonName)
        {
            Context db = (Context)context;

            return db.Users.Where(p => p.LogonName == logonName).FirstOrDefault();
        }

        public bool UserExists(string logonName)
        {
            return this.dbSet.Any(p => p.LogonName == logonName);
        }

        public bool EditUserExists(string logonName)
        {
            return this.dbSet.Where(p => p.LogonName == logonName).Count() > 1;
        }

        public User Verify(string logonName, string password)
        {
            Context db = (Context)context;

            return db.Users.Where(p => p.LogonName == logonName && p.Password == password).FirstOrDefault();
        }
    }
}