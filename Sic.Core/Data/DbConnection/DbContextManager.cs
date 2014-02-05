using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Sic.Data.Models.Security;
using Sic.Data.Models.General;

namespace Sic.Data.DbConnection
{
    public class DbContextManager: Sic.Data.Entity.DbContext
    {
        public DbContextManager(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<UserBase> Users { get; set; }
        public DbSet<UserLoginAuditBase> UserLoginAudits { get; set; }
        public DbSet<UserRoleBase> UserRoles { get; set; }
        public DbSet<RoleBase> Roles { get; set; }
        public DbSet<ContactBase> Contacts { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactBase>().HasKey(p => p.ContactId);
            modelBuilder.Entity<UserBase>().HasKey(p => p.UserId);

            modelBuilder.Entity<ContactBase>().HasOptional(p => p.User).WithRequired(p => p.Contact);

            //modelBuilder.Entity<UserBase>().HasRequired(p => p.Contact).WithOptional(p=>p.User);            
            //modelBuilder.Entity<ContactBase>().HasOptional(p => p.User).WithRequired(p=>p.Contact);
            modelBuilder.Entity<ContactBase>().HasMany(p => p.ContactUs).WithRequired(p => p.Contact);
            //modelBuilder.Entity<UserBase>().HasRequired(p => p.Contact).WithRequiredDependent(p=>p.User);
        }
    }
}
