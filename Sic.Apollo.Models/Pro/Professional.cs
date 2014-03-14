using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using Sic.Data.Entity;
using System.Web.Mvc;
using Sic.Web.Mvc.Validation;
using Sic.Apollo.Models.Medical;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Pro
{    
    [Table("tbProfessional", Schema = "pro")]
    public class Professional : Sic.Data.Entity.EntityBase
    {               
        [Key]        
        public int ProfessionalId { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForProfessionalDescription")]        
        public string ProfessionalDescription { get; set; }

        public int? RateScore { get; set; }

        public int? RateScore1 { get; set; }

        public int? RateScore2 { get; set; }

        public int? RateScore3 { get; set; }

        public override string DescriptionName
        {
            get
            {
                return string.Format("{0} {1}", (this.Contact.Gender == (int)Gender.Female ?
                        Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort :
                        Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort), Contact.FullName);
            }
        }

        public string DescriptionName2
        {
            get
            {
                return string.Format("{0} {1}", (this.Contact.Gender == (int)Gender.Female ?
                        Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort :
                        Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort), Contact.FullName2);
            }
        }

        public string DescriptionName3
        {
            get
            {
                return string.Format("{0} {1}",(this.Contact.Gender == (int)Gender.Female?
                        Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort:
                        Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort), Contact.FullName3);
            }
        }

        public string Specializations
        {
            get
            {
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                if (this.ProfessionalSpecializations.Any())
                {                    
                    foreach (ProfessionalSpecialization sp in this.ProfessionalSpecializations)
                    {
                        if(result.Length>0)
                            result.Append(" - ");
                        result.Append(sp.Specialization.Profession);
                    }
                }
                
                return result.ToString();                
            }
        }

        public string UrlParameter
        {
            get
            {
                return string.Format("{0}-{1}", this.Contact.FullName.Replace(",", "").Replace(" ", "-"), this.ProfessionalId);
            }
        }

        #region Navigation Properties

        public virtual Contact Contact  { get; set; }

        public virtual List<ProfessionalSpecialization> ProfessionalSpecializations { get; set; }
        
        public virtual List<ProfessionalOffice> ProfessionalOffices { get; set; }

        public virtual List<ProfessionalSchool> ProfessionalSchools { get; set; }

        public virtual List<ProfessionalCommunity> ProfessionalCommunities { get; set; }

        public virtual List<ProfessionalExperience> ProfessionalExperiences { get; set; }

        public virtual List<ProfessionalInsuranceInstitutionPlan> ProfessionalInsuranceInstitutionPlans { get; set; }

        public virtual List<ProfessionalPatient> ProfessionalPatients { get; set; }

        public virtual List<Appointment.Appointment> Appointments { get; set; }

        public virtual List<MedicalCare> MedicalCares { get; set; }

        public virtual List<PatientVitalSign> PatientVitalSigns { get; set; }

        public virtual List<MedicalHistory> MedicalHistories { get; set; }

        public virtual List<ProfessionalVademecum> ProfessionalVademecums { get; set; }

        public virtual List<ProfessionalTeam> ProfessionalTeam { get; set; }

        #endregion

        public override void OnCreate()
        {
            base.OnCreate();

            this.Contact = new Contact();
            this.Contact.Gender = (int)Gender.Male;            
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(this.Contact.PhoneNumber) && string.IsNullOrWhiteSpace(this.Contact.CellPhone))
            {
                modelState.AddModelError("Contact.PhoneNumber", Sic.Apollo.Resources.Resources.ValidationForAnyPhoneNumber);
            }
            //if (string.IsNullOrWhiteSpace(this.ProfessionalDescription))
            //{
            //    modelState.AddModelError("ProfessionalDescription", Sic.Apollo.Resources.Resources.ValidationFieldRequired);
            //}
            if (this.ProfessionalSpecializations == null || !this.ProfessionalSpecializations.Any())
            {
                modelState.AddModelError("ProfessionalSpecializations", Sic.Apollo.Resources.Resources.ValidationForRequiredProfessionalSpecializations);
            }

            return Contact.Validate(modelState, true);
        }        
    }
}