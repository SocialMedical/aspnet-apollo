using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Data.Entity;
using Sic.Data.Models.Security;
using Sic.Data.Models.General;

namespace Sic.Data.DbConnection
{
    public class DbContextManagerService<T> : Sic.Data.Entity.DbContextService<T> where T : DbContext, IDisposable
    {        
        public DbContextManagerService(string connectionStringKey, bool isEncryptedConnectionString)
            : base(connectionStringKey, isEncryptedConnectionString)
        {
        }

        private Repository<UserBase> userRepository;
        private Repository<OrganizationBase> organizationRepository;
        private Repository<UserRoleBase> userRoleRepository;
        private Repository<RoleBase> roleRepository;
        private Repository<UserLoginAuditBase> userLoginAuditeRepository;
        private Repository<ContactBase> contactRepository;
        private Repository<ContactUs> contactUsRepository;

        public Repository<UserBase> Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new Repository<UserBase>(Context);
                return userRepository;
            }
        }

        public Repository<UserLoginAuditBase> UserLoginAudits
        {
            get
            {
                if (userLoginAuditeRepository == null)
                    userLoginAuditeRepository = new Repository<UserLoginAuditBase>(Context);
                return userLoginAuditeRepository;
            }
        }

        public Repository<ContactBase> Contacts
        {
            get
            {
                if (contactRepository == null)
                    contactRepository = new Repository<ContactBase>(Context);
                return contactRepository;
            }
        }

        public Repository<RoleBase> Roles
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = new Repository<RoleBase>(Context);
                return roleRepository;
            }
        }

        public Repository<OrganizationBase> Organizations
        {
            get
            {
                if (organizationRepository == null)
                    organizationRepository = new Repository<OrganizationBase>(Context);
                return organizationRepository;
            }
        }

        public Repository<UserRoleBase> UserRoles
        {
            get
            {
                if (userRoleRepository == null)
                    userRoleRepository = new Repository<UserRoleBase>(Context);
                return userRoleRepository;
            }
        }

        public Repository<ContactUs> ContactUs
        {
            get
            {
                if (contactUsRepository == null)
                    contactUsRepository = new Repository<ContactUs>(Context);
                return contactUsRepository;
            }
        }        
       
    }

    public class DbContextManagerService : Sic.Data.DbConnection.DbContextManagerService<DbContextManager>, IDisposable
    {
        public DbContextManagerService()
            : base("ManagerDb", true)
        {
        }
    }
}
