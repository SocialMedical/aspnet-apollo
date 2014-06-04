using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sic.Configuration
{
    public class SicConfigurationSection : ConfigurationSection
    {
        public static SicConfigurationSection Current
        {
            get
            {
                return (SicConfigurationSection)
                    System.Configuration.ConfigurationManager.GetSection("sicConfigurationGroup/sicConfigurationSection");
            }
        }

        [ConfigurationProperty("fileImportVersion", DefaultValue = "1000", IsRequired = true)]
        [StringValidator(MaxLength = 5)]
        public String FileImportVersion
        {
            get
            {
                return (String)this["fileImportVersion"];
            }
            set
            {
                this["fileImportVersion"] = value;
            }
        }

        [ConfigurationProperty("mailServer")]
        public String MailServer
        {
            get
            {
                return (String)this["mailServer"];
            }
            set
            {
                this["mailServer"] = value;
            }
        }        
        
        [ConfigurationProperty("emailAgent")]
        public AccountElement EmailAgent
        {
            get
            {
                return (AccountElement)this["emailAgent"];
            }
            set
            {
                this["emailAgent"] = value;
            }
        }

        [ConfigurationProperty("mainContent")]
        public ImportElement MainContent
        {
            get
            {
                return (ImportElement)this["mainContent"];
            }
            set
            {
                this["mainContent"] = value;
            }
        }

        [ConfigurationProperty("image")]
        public ImportElement Image
        {
            get
            {
                return (ImportElement)this["image"];
            }
            set
            {
                this["image"] = value;
            }
        }

        [ConfigurationProperty("styleSheet")]
        public ImportElement StyleSheet
        {
            get
            {
                return (ImportElement)this["styleSheet"];
            }
            set
            {
                this["styleSheet"] = value;
            }
        }

        [ConfigurationProperty("script")]
        public ImportElement Script
        {
            get
            {
                return (ImportElement)this["script"];
            }
            set
            {
                this["script"] = value;
            }
        }

        [ConfigurationProperty("logInRedirect")]
        public RedirectionPageElement LogInRedirect
        {
            get
            {
                return (RedirectionPageElement)this["logInRedirect"];
            }
            set
            {
                this["logInRedirect"] = value;
            }
        }

        [ConfigurationProperty("homeRedirect")]
        public RedirectionPageElement HomeRedirect
        {
            get
            {
                return (RedirectionPageElement)this["homeRedirect"];
            }
            set
            {
                this["homeRedirect"] = value;
            }
        }

        [ConfigurationProperty("logOutRedirect")]
        public RedirectionPageElement LogOutRedirect
        {
            get
            {
                return (RedirectionPageElement)this["logOutRedirect"];
            }
            set
            {
                this["logOutRedirect"] = value;
            }
        }

        [ConfigurationProperty("accessDeniedRedirect")]
        public RedirectionPageElement AccessDeniedRedirect
        {
            get
            {
                return (RedirectionPageElement)this["accessDeniedRedirect"];
            }
            set
            {
                this["accessDeniedRedirect"] = value;
            }
        }

        [ConfigurationProperty("expiredSessionRedirect")]
        public RedirectionPageElement ExpiredSessionRedirect
        {
            get
            {
                return (RedirectionPageElement)this["expiredSessionRedirect"];
            }
            set
            {
                this["expiredSessionRedirect"] = value;
            }
        }

        [ConfigurationProperty("registerRedirect")]
        public RedirectionPageElement RegisterRedirect
        {
            get
            {
                return (RedirectionPageElement)this["registerRedirect"];
            }
            set
            {
                this["registerRedirect"] = value;
            }
        }
    }    

    public class ImportElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        [StringValidator]
        public String Path
        {
            get
            {
                return (String)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }
    }

    public class AccountElement : ConfigurationElement
    {
        [ConfigurationProperty("email", IsRequired = true)]
        [StringValidator]
        public String Email
        {
            get
            {
                return (String)this["email"];
            }
            set
            {
                this["email"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        [StringValidator]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("password")]
        [StringValidator]
        public String Password
        {
            get
            {
                return (String)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
    }

    public class RedirectionPageElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        [StringValidator]
        public String Url
        {
            get
            {
                return (String)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }
    }
}
