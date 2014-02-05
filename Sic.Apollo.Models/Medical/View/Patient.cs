using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Web.Mvc;

namespace Sic.Apollo.Models.Medical.View
{
    public class Patient
    {        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]        
        public int PatientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFirstName")]        
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
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

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDefaultCellphone")]
        public string CellPhone { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
