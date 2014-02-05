using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Resources;
using Sic.Data.Entity;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Models.General
{
    [Table("tbContactLocation", Schema="gen")]
    public class ContactLocation : EntityBase, IContactLocation
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]       
        [Key]        
        public int ContactLocationId { get; set; }
                
        public int ContactId { get; set; }

        [StringLength(100)]
        //[Required(ErrorMessageResourceType=typeof(Resources.Resources),ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDescriptionAddress")]
	    public string Description { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldRequired")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPriority")]
        public int Priority { get; set; }
        
        public int LocationType { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForCountry")]
        public int CountryId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForState")]
        public int StateId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForCity")]
        public int CityId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForArea")]
        public int? AreaId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLatitudePoint")]
        public double? Latitude { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLongitudePoint")]
        public double? Longitude { get; set; }

        [StringLength(500)]
        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAddress")]
        public string Address { get; set; }               

        [StringLength(255)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForReferenceAddress")]
        public string References { get; set; }

        [StringLength(50)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPostalCode")]
        public string PostalCode { get; set; }

        [StringLength(50)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPOBox")]
        public string POBox { get; set; }

        [Required]
        [StringLength(20)]        
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDefaultPhoneNumber")]
        public string DefaultPhoneNumber { get; set; }

        [StringLength(6)]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForExtensionPhoneNumber")]
        public string DefaultPhoneExtension { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoneNumber")]
        public string PhoneNumber01 { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForExtensionPhoneNumber")]
        public string PhoneExtension01 { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForPhoneNumber")]        
        public string PhoneNumber02 { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForExtensionPhoneNumber")]
        public string PhoneExtension02 { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForEmail")]
        public string Email { get; set; }
                        

        public string DefaultPhoneNumberDescription
        {
            get{
                return String.Format("{0}: {1} {2} {3}", Sic.Apollo.Resources.Resources.LabelForPhoneNumber, this.DefaultPhoneNumber,
                                string.IsNullOrWhiteSpace(this.DefaultPhoneExtension) ? "" : Sic.Apollo.Resources.Resources.LabelForExtensionPhoneNumber, 
                                this.DefaultPhoneExtension);
            }
        }

        public string PhoneNumber01Description
        {
            get
            {
                return String.Format("{0} {1} {2}", this.PhoneNumber01,
                                string.IsNullOrWhiteSpace(this.PhoneExtension01) ? "" : Sic.Apollo.Resources.Resources.LabelForExtensionPhoneNumber,
                                this.PhoneExtension01);
            }
        }

        public string PhoneNumbersDescription
        {
            get
            {
                return DefaultPhoneNumberDescription + (string.IsNullOrEmpty(this.PhoneNumber01) ? "" : this.PhoneNumber01Description);
            }
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLatitudePoint")]
        [NotMapped]
        public string LatitudeString { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLongitudePoint")]
        [NotMapped]
        public string LongitudeString { get; set; }

        [NotMapped]
        public int MarkerIndex { get; set; }
        [NotMapped]
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        [NotMapped]
        public int MarkerStart { get { return 30 * (MarkerIndex - 1); } }
        #region Navigation Properties                                

        public virtual List<ContactLocationPicture> ContactLocationPictures { get; set; }

        public virtual Country Country { get; set; }

        public virtual State State { get; set; }

        public virtual City City { get; set; }

        public virtual Area Area { get; set; }
        
        #endregion
    }
}