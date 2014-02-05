using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Pro.View
{
    public class Professional: EntityBase, IContactLocation
    {
        public string PictureMin { get; set; }
        public int MarkerIndex { get; set; }
        public int MarkerStart { get { return 30 * (MarkerIndex -1); } }
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? RateScore { get; set; }
        public int? RateScore1 { get; set; }
        public int? RateScore2 { get; set; }
        public int? RateScore3 { get; set; }
        public DateTime RegisterDate { get; set; }
        public int UserState { get; set; }

        public int Gender { get; set; }

        public int ProfessionalId { get; set; }
        public int CityId { get; set; }
        public int SpecializationId { get; set; }
        public int ContactLocationId { get; set; }        

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForSpecialization")]
        public string SpecializationName { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForMiddleName")]
        public string MiddleName { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForLastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForSecondLastName")]
        public string SecondLastName { get; set; }                                

        public string Picture { get; set; }

        public string DefaultPhoneNumber { get; set; }

        public string DefaultPhoneExtension { get; set; }

        public string UrlParameter
        {
            get
            {
                return string.Format("{0}-{1}", this.FullName.Replace(",", "").Replace(" ", "-"), this.ProfessionalId);
            }
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFullName")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public override string DescriptionName
        {
            get
            {
                return string.Format("{0} {1}", (Gender)this.Gender == Sic.Gender.Female ?
                    Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort, this.FullName);
            }
        }

        public string DescriptionInThirdPerson
        {
            get
            {
                return string.Format("{0} {1}", (Gender)this.Gender == Sic.Gender.Female ?
                    Sic.Apollo.Resources.Resources.LabelForShe : Sic.Apollo.Resources.Resources.LabelForHe, this.DescriptionName);
            }
        }

        public string ProfessionalDescription { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAddress")]
        public string Address
        {
            get;
            set;
        }
    }
}