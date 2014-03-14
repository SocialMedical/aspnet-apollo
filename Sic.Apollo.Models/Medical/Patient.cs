using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPatient", Schema="med")]
    public class Patient : Sic.Data.Entity.EntityBase
    {
        [Key]
        public int PatientId { get; set; }

        public byte BloodGroup { get; set; }

        public string Comments { get; set; }

        public string DescriptionName3
        {
            get
            {
                return this.Contact.FullName3;
            }
        }

        public virtual Contact Contact { get; set; }

        public virtual List<ProfessionalPatient> ProfessionalPatients { get; set; }

        public virtual List<PatientInsuranceInstitution> PatientInsuranceInstitutions { get; set; }

        public virtual List<MedicalCare> MedicalCares { get; set; }

        public virtual List<PatientVitalSign> PatientVitalSigns { get; set; }

        public virtual List<MedicalHistory> MedicalHistories { get; set; }

        public virtual List<PatientPhysicalExamination> PatientPhysicalExaminations { get; set; }

        public virtual List<PatientFile> PatientFiles { get; set; }

        public bool Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            return Contact.Validate(modelState, true);            
        }

        private List<MedicalCare> resumeMedicalCares;
        [NotMapped]
        public List<MedicalCare> ResumeMedicalCares
        {
            get
            {
                if (resumeMedicalCares == null) resumeMedicalCares = new List<MedicalCare>();
                return resumeMedicalCares;
            }
        }

        private List<PatientVitalSign> resumePatientVitalSigns;
        [NotMapped]
        public List<PatientVitalSign> ResumePatientVitalSigns
        {
            get
            {
                if (resumePatientVitalSigns == null) resumePatientVitalSigns = new List<PatientVitalSign>();
                return resumePatientVitalSigns;
            }
        }

        private List<PatientPhysicalExamination> resumePatientPhysicalExaminations;
        [NotMapped]
        public List<PatientPhysicalExamination> ResumePatientPhysicalExamination
        {
            get
            {
                if (resumePatientPhysicalExaminations == null) resumePatientPhysicalExaminations = new List<PatientPhysicalExamination>();
                return resumePatientPhysicalExaminations;
            }
        }

        private List<MedicalHistory> resumeMedicalHistories;
        [NotMapped]
        public List<MedicalHistory> ResumeMedicalHistories
        {
            get
            {
                if(resumeMedicalHistories == null) resumeMedicalHistories = new List<MedicalHistory>();
                return resumeMedicalHistories;
            }
        }

    }
}
