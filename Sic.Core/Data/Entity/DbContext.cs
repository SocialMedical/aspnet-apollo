using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.Entity
{
    public class DbContext: System.Data.Entity.DbContext
    {

        public DbContext(string connectionString)
            : base(connectionString)
        {

        }

        public int ExecuteSqlCommand(string sql, List<Parameter> parameters = null)
        {
            sql = Parameter.ApplyParameters(sql, parameters);
            return base.Database.ExecuteSqlCommand(sql);
        }

        List<string> pendingCommands = null;
        public List<string> PendingCommands
        {
            get
            {                
                if(pendingCommands == null)
                    pendingCommands = new List<string>();
                return pendingCommands;
            }
        }

        public override int SaveChanges()
        {
            int result = 0;
            foreach (string sql in PendingCommands)
                result = ExecuteSqlCommand(sql);
            
            int resultDb = base.SaveChanges();
            return result + resultDb;
        }
    }
}
