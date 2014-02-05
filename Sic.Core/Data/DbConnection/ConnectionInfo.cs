using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.DbConnection
{
    public class ConnectionInfo
    {
        private string providerName;
        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
            set
            {
                this.providerName = value;
                AssignConnectionString();
            }
        }

        private string dataBase;
        public string DataBase
        {
            get
            {
                return dataBase;
            }
            set
            {
                dataBase = value;
                AssignConnectionString();
            }
        }

        private string userId;
        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {                
                userId = value;
                AssignConnectionString();
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                AssignConnectionString();
            }
        }

        private string dataSource;
        public string DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                this.dataSource = value;
                AssignConnectionString();
            }
        }

        private string connectionString;
        public string ConnectionString 
        { 
            get 
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                AssignProperties();
            }
        }

        private void AssignConnectionString()
        {
            this.connectionString = string.Format("Data Source={0};Initial Catalog={1};user Id={2};password={3}", 
                this.DataSource, this.DataBase, this.UserId, this.Password);
        }

        private void AssignProperties()
        {           
            string[] connectionParams = connectionString.Split(';');
            
            string dataSourceEntry = connectionParams.FirstOrDefault(p => p.ToLower().Contains("data source"));
            string[] dataSourceParts = dataSourceEntry.Split('=');
            this.dataSource = dataSourceParts[1];

            string dataBaseEntry = connectionParams.FirstOrDefault(p => p.ToLower().Contains("initial catalog"));
            string[] dataBaseParts = dataBaseEntry.Split('=');
            this.dataBase = dataBaseParts[1];

            string userEntry = connectionParams.FirstOrDefault(p => p.ToLower().Contains("user id"));
            this.userId = userEntry.Substring(userEntry.IndexOf('=')+1);

            string passwordEntry = connectionParams.FirstOrDefault(p => p.ToLower().Contains("password"));
            this.password = passwordEntry.Substring(passwordEntry.IndexOf('=') + 1);
        }
    }
}
