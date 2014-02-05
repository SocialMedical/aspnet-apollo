using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models.General.View
{
    public class Person
    {
        public int ContactId { get; set; }
                
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string MiddleName { get; set; }
        
        public string SecondLastName { get; set; }

        public string FullName
        {
            get
            {                
                return string.Format("{0}{1}, {2}{3}",                     
                    string.IsNullOrWhiteSpace(this.LastName)?"":this.LastName, 
                    string.IsNullOrWhiteSpace(this.SecondLastName)?"":" " + this.SecondLastName,
                    string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName, 
                    string.IsNullOrWhiteSpace(this.MiddleName)?"":" " + this.MiddleName);
            }
        }
    }
}
