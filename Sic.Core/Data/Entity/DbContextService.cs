using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Data.Entity;
using System.Reflection;

namespace Sic.Data.Entity
{
    public class DbContextService<T> : IDisposable, IDbContextService where T : DbContext
    {
        private string ConnectionString;

        protected T Context;

        public DbContextService(string connectionStringKey, bool isEncryptedConnectionString)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringKey].ProviderName;
            DbConnection.ConnectionInfo connection = new DbConnection.ConnectionInfo();

            if (isEncryptedConnectionString)
            {
                connection.ConnectionString = connectionString;
                connection.UserId = Sic.Security.Cryptography.Decrypt(connection.UserId);
                connection.Password = Sic.Security.Cryptography.Decrypt(connection.Password);
                this.ConnectionString = connection.ConnectionString;
            }
            else
            {
                this.ConnectionString = connectionString;
            }

            ConstructorInfo constructor = typeof(T).GetConstructor(new Type[] { typeof(string) });
            Context = constructor.Invoke(new object[] { (string)this.ConnectionString }) as T;

            Context.Configuration.AutoDetectChangesEnabled = false;
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
