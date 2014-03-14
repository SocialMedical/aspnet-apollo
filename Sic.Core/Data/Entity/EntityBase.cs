using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Data.Entity
{
    public abstract class EntityBase: IIdentifiable
    {
        #region IIdentifiable Members

        [NotMapped()]
        public virtual string Key { get { return string.Empty; } }

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
