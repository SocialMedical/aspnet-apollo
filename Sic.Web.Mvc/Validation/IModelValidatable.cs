using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc.Validation
{
    public interface IModelValidatable
    {
        bool Validate(ModelStateDictionary modelState);
    }
}
