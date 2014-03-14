using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using System.Web.Mvc;
using Sic.Web.Mvc;
using Sic.Apollo.Models.Pro;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Security
{
    [Table("tbUser", Schema = "sec")]
    public class User : Sic.Data.Entity.EntityBase
    {
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]        
        public int UserId { get; set; }
        
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldForDataType")]
        [Remote("ValidateLogonName", "Account", ErrorMessageResourceType = typeof(Sic.Apollo.Resources.Resources), ErrorMessageResourceName = "ValidationForDuplicateAccountLogonName")]
        public string LogonName { get; set; }        

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPassword")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMaximunLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMinimunLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        private string confirmedPassword = null;
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForConfirmedPassword")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMaximunLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForMinimunLength")]
        [DataType(DataType.Password)]
        [NotMapped]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationForComparePassword")]
        public string ConfirmedPassword
        {
            get
            {
                if (confirmedPassword == null)
                    confirmedPassword = this.Password;
                return confirmedPassword;
            }
            set
            {
                confirmedPassword = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForUserState")]  
        public int State { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForUserType")]  
        public int Type { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForRegisterDate")]  
        public DateTime RegisterDate { get; set; }
        
        public DateTime? ConfirmationDate { get; set; }

        [ForeignKey("UserId")]
        public virtual Contact Contact { get; set; }

        public virtual List<ProfessionalTeam> ProfessionalTeam { get; set; } 

        public string StateDisplay { get { return this.State.GetDisplay(typeof(UserState)); } }

        public string TypeDisplay { get { return this.Type.GetDisplay(typeof(UserType)); } }                

        public bool Validate(ModelStateDictionary modelState)
        {
            ContextService db = new ContextService();
            if (this.UserId == 0)
            {
                if (db.Users.UserExists(this.LogonName))
                    modelState.AddModelError("LogonName", Sic.Apollo.Resources.Resources.ValidationForDuplicateAccountLogonName);
            }
            else
            {
                User current = db.Users.GetByID(this.UserId);
                if (current.LogonName != this.LogonName)
                    if (db.Users.UserExists(this.LogonName))
                        modelState.AddModelError("LogonName", Sic.Apollo.Resources.Resources.ValidationForDuplicateAccountLogonName);
            }
            return modelState.IsValid;
        }
    }

    public class SignIn
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForEmail")]
        public string LogonName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPassword")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForRememberMe")]
        public bool RememberMe { get; set; }

        public LoginAction LoginFor { get; set; } 

        public enum LoginAction
        {
            Appointment,            
            Login,
            PopUp,
            ExpiredSession
        }
    }    
}