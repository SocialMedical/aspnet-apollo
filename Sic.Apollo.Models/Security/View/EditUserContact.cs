using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Web.Mvc;
using System.Web.Mvc;
using Sic.Apollo.Models.General;
using Sic.Web.Mvc.Entity;

namespace Sic.Apollo.Models.Security.View
{
    public class EditUserContact: ModelEntity
    {
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int UserId { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldForDataType")]        
        public string LogonName { get; set; }        

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForUserType")]
        public int Type { get; set; }

        [NotMapped]
        public int ProfessionalTeamId { get; set; }

        public Contact Contact { get; set; }

        public override bool Validate(ModelStateDictionary modelState)
        {
            ContextService db = new ContextService();
            
            User current = db.Users.GetByID(this.UserId);
            if (current.LogonName != this.LogonName)
                if (db.Users.UserExists(this.LogonName))
                    modelState.AddModelError("LogonName", Sic.Apollo.Resources.Resources.ValidationForDuplicateAccountLogonName);
            
            return base.Validate(modelState);
        }
    }
}
