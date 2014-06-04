using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data
{
    /// <summary>
    /// Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IDAttribute : Attribute
    {        
        string id;

        public IDAttribute(string IDvalue)
        {
            this.IdValue = IDvalue;
        }

        /// <summary>
        /// Get and set.
        /// </summary>
        public string IdValue
        {
            get { return this.id; }
            set { this.id = value; }
        }
    }
}
