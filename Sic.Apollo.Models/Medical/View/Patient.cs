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
        public int PatientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.MessageFor), ErrorMessageResourceName = "ValidationFieldRequired")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessageResourceType = typeof(Resources.MessageFor), ErrorMessageResourceName = "ValidationFieldRequired")]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string SecondLastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.MessageFor), ErrorMessageResourceName = "ValidationFieldRequired")]
        public byte Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", /*NullDisplayText = "Ingrese su Fecha de Nacimiento",*/ ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.LabelFor), Name = "BirthDate")]
        public DateTime? BirthDate { get; set; }

        private int? birthDateYear;
        [Range(1900, 2050)]
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
        public int? BirthDateMonth
        {
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
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        [Range(1, 31)]
        public int? BirthDateDay
        {
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

        [StringLength(100)]
        [Email(ErrorMessageResourceType = typeof(Resources.MessageFor), ErrorMessageResourceName = "ValidationFieldForDataType")]
        public string Email { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string BirthPlace { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        public string CellPhone { get; set; }

        public int? Age
        {
            get
            {
                if (this.BirthDate.HasValue)
                {
                    DateTime now = DateTime.Now;
                    int age = now.Year - this.BirthDate.Value.Year;
                    if (now < this.BirthDate.Value.AddYears(age)) age--;
                    return age;
                }
                return null;
            }
        }
        public string FullName
        {
            get
            {                
                return string.Format("{0}{1}{2}", this.FirstName, " " + this.LastName, " " + this.SecondLastName);                
            }
        }

        public string Comments
        {
            get;
            set;
        }

        public byte BloodGroup
        {
            get;
            set;
        }

        public Models.Medical.Patient PatientModel { get; set; }

        public virtual List<PatientInsuranceInstitution> PatientInsuranceInstitutions { get; set; }

        public virtual List<MedicalCare> MedicalCares { get; set; }

        public virtual List<PatientVitalSign> PatientVitalSigns { get; set; }

        public virtual List<MedicalHistory> MedicalHistories { get; set; }

        public virtual List<PatientPhysicalExamination> PatientPhysicalExaminations { get; set; }

        public virtual List<PatientFile> PatientFiles { get; set; }


        private List<MedicalCare> resumeMedicalCares;    
        public List<MedicalCare> ResumeMedicalCares
        {
            get
            {
                if (resumeMedicalCares == null) resumeMedicalCares = new List<MedicalCare>();
                return resumeMedicalCares;
            }
        }

        private List<PatientVitalSign> resumePatientVitalSigns;        
        public List<PatientVitalSign> ResumePatientVitalSigns
        {
            get
            {
                if (resumePatientVitalSigns == null) resumePatientVitalSigns = new List<PatientVitalSign>();
                return resumePatientVitalSigns;
            }
        }

        private List<PatientPhysicalExamination> resumePatientPhysicalExaminations;        
        public List<PatientPhysicalExamination> ResumePatientPhysicalExamination
        {
            get
            {
                if (resumePatientPhysicalExaminations == null) resumePatientPhysicalExaminations = new List<PatientPhysicalExamination>();
                return resumePatientPhysicalExaminations;
            }
        }

        private List<MedicalHistory> resumeMedicalHistories;        
        public List<MedicalHistory> ResumeMedicalHistories
        {
            get
            {
                if (resumeMedicalHistories == null) resumeMedicalHistories = new List<MedicalHistory>();
                return resumeMedicalHistories;
            }
        }
    }
}
