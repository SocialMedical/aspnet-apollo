using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sic.Apollo.Models.Security.View
{
    public class ChangePassword
    {
        public int UserId { get; set; }
        
        public string LogonName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPassword")]
        [DataType(DataType.Password)]        
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForNewPassword")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMaximunLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMinimunLength")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LableForConfirmNewPassword")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMaximunLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMinimunLength")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForComparePassword")]
        public string ConfirmedNewPassword { get; set; }
    }
}
