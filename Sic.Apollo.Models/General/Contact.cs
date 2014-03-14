using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.Pro;
using Sic.Data.Entity;
using Sic.Apollo.Resources;
using System.Web.Mvc;
using Sic.Apollo.Models.Security;
using Sic.Web.Mvc;
using System.Security.AccessControl;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.General
{
    [Table("tbContact", Schema = "gen")]
    public class Contact : Sic.Data.Entity.EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        [ScaffoldColumn(false)]
        public int ContactId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFirstName")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [RequiredIf("IsPerson", true, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLastName")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForMiddleName")]
        public string MiddleName { get; set; }

        [StringLength(50)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForSecondLastName")]
        public string SecondLastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForGender")]
        public byte Gender { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", /*NullDisplayText = "Ingrese su Fecha de Nacimiento",*/ ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForBirthDate")]
        public DateTime? BirthDate { get; set; }

        #region BirthDate

        public void AssingDateParts()
        {
            if (this.BirthDate != null)
            {
                this.BirthDateYear = this.BirthDate.Value.Year;
                this.BirthDateMonth = this.BirthDate.Value.Month;
                this.BirthDateDay = this.BirthDate.Value.Day;
            }
        }

        private int? birthDateYear;

        [NotMapped()]
        [Range(1900, 2050)]
        //[Required]
        public int? BirthDateYear
        {
            get
            {
                if (!birthDateYear.HasValue && this.BirthDate.HasValue)
                    birthDateYear = BirthDate.Value.Year;
                return birthDateYear;
            }
            set
            {
                birthDateYear = value;
            }
        }


        private int? birthDateMonth;
        [NotMapped()]               
        public int? BirthDateMonth {
            get
            {
                if (!birthDateMonth.HasValue && this.BirthDate.HasValue)
                    birthDateMonth = BirthDate.Value.Month;
                return birthDateMonth;
            }
            set
            {
                birthDateMonth = value;
            }
        }

        private int? birthDateDay = null;
        [NotMapped()]        
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        //[Required]
        [Range(1,31)]        
        public int? BirthDateDay {
            get
            {
                if (!birthDateDay.HasValue && this.BirthDate.HasValue)
                    birthDateDay = BirthDate.Value.Day;
                return birthDateDay;
            }
            set
            {
                birthDateDay = value;
            }
        }

        #endregion BirthDate

        [StringLength(100)]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldForDataType")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDefaultEmail")]
        public string Email { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoto")]
        public string Picture { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForBirthPlace")]
        public string BirthPlace { get; set; }        

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoto")]
        public string PictureMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.Picture))
                    return string.Empty;
                return Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(this.Picture);
            }
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoto")]
        public string PictureMedium
        {
            get
            {
                if (string.IsNullOrEmpty(this.Picture))
                    return string.Empty;
                return Sic.Web.Mvc.Utility.Thumbnail.GetPictureMediumFromOriginal(this.Picture);
            }
        }

        [StringLength(100)]
        [DataType(DataType.Url)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDefaultWebSite")]
        public string WebSite { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDefaultCellphone")]
        public string CellPhone { get; set; }

        [StringLength(100)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForTwitterAccount")]
        public string TwitterAccount { get; set; }

        [StringLength(100)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFacebookAccount")]
        public string FacebookAccount { get; set; }

        [StringLength(100)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForGooglePlusAccount")]
        public string GooglePlusAccount { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFullName")]
        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return FirstName + " " + LastName;
                else
                    return FirstName;
            }
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFullName")]
        public string FullName3
        {
            get
            {
                return string.Format("{0}{1}{2}{3}",                    
                    string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName,
                    string.IsNullOrWhiteSpace(this.MiddleName) ? "" : " " + this.MiddleName,
                    string.IsNullOrWhiteSpace(this.LastName) ? "" : " " + this.LastName,
                    string.IsNullOrWhiteSpace(this.SecondLastName) ? "" : " " + this.SecondLastName);
            }
        }

        [NotMapped]
        public bool IsPerson
        {
            get;
            set;
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFullName")]
        public string FullName2
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return FirstName + " " + LastName;
                else
                    return FirstName;
            }
        }

        public int? Age
        {
            get
            {
                if(this.BirthDate.HasValue)
                {
                    DateTime now = DateTime.Now;
                    int age = now.Year - this.BirthDate.Value.Year;
                    if (now < this.BirthDate.Value.AddYears(age)) age--;
                    return age;
                }
                return null;
            }
        }

        public string GenderDisplay { get { return this.Gender.GetDisplay(typeof(Gender)); } }

        #region Navigation Properties

        public virtual List<ContactLocation> ContactLocations { get; set; }

        #endregion

        public bool Validate(ModelStateDictionary modelState)
        {
            return Validate(modelState);
        }

        public bool Validate(ModelStateDictionary modelState, bool isPersonContact = true)
        {
            bool error = false;
            
            try
            {
                if (isPersonContact && string.IsNullOrWhiteSpace(this.LastName))
                {
                    modelState.AddModelError("Contact.LastName", Sic.Apollo.Resources.Resources.ValidationFieldRequired);
                }
                if (this.BirthDateYear != null || this.BirthDateMonth != null || this.BirthDateDay != null)
                {
                    try
                    {
                        this.BirthDate = new DateTime(this.BirthDateYear.Value, this.BirthDateMonth.Value, this.BirthDateDay.Value);
                        if (this.BirthDateYear.Value > Sic.Web.Mvc.Session.CurrentDateTime.Year)
                        {
                            modelState.AddModelError("Contact.BirthDate", Sic.Apollo.Resources.Resources.ValidationFieldForDataType);
                        }
                    }
                    catch (Exception)
                    {
                        modelState.AddModelError("Contact.BirthDate", Sic.Apollo.Resources.Resources.ValidationFieldForDataType);
                        error = true;
                    }
                }

                return true;
            }
            catch
            {
                error = false;
            }
            return error;
        }      
    }
}