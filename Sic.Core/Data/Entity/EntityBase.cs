using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sic.Data.Entity
{
    public abstract class EntityBase: IIdentifiable
    {
        #region IIdentifiable Members

        [NotMapped()]
        public virtual int Key { get { return 0; } }

        [NotMapped()]
        public virtual string DescriptionName { get { return string.Empty; } }

        #endregion  
       
        public virtual void OnCreate()
        {
        }

        public List<Parameter> GetParameters(string[] exludeProperties = null)
        {
            return Sic.Data.Parameter.GetPropertiesParameters(this, exludeProperties);
        }
    }
}
