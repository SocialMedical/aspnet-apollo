using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;

namespace Sic.Web.Mvc
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class EmailAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public EmailAttribute()
            : base(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")
        {
            //this.ErrorMessage = "Please provide a valid email address";
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(base.ErrorMessageString))
                return String.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name);
            else
                return "Please provide a valid email address";
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "email"
            };

            return new[] { clientValidationRule };
        }

        #endregion
    }


}
