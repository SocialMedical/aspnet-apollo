using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Web.Mvc.Validation;
using System.Web.Mvc;

namespace Sic.Web.Mvc.Entity
{
    public abstract class ModelEntity : Sic.Data.Entity.EntityBase, IModelValidatable
    {
        public virtual bool Validate(ModelStateDictionary modelState)
        {
            return true;
        }
    }
}
