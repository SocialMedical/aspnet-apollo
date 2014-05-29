using Microsoft.Reporting.WebForms;
using Sic.Apollo.Controllers;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Apollo.Models.Medical.View;
using Sic.Apollo.Models.Pro;
using Sic.Web.Mvc;
using Sic.Web.Mvc.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class MedicalCareController : BaseController
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Detail(long medicalCareId, int patientId)
        {
            if (!IsValidProfesionalPatient(patientId, true)) return Json();
            var history = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
            return PartialView("_MedicalCare", history);
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Edit(long medicalCareId, int patientId)
        {
            if (IsValidProfesionalPatient(patientId, true))
            {
                try
                {
                    MedicalCare medicalCare = null;
                    if (medicalCareId == 0)
                    {
                        medicalCare = new Models.Medical.MedicalCare();
                        medicalCare.EvolutionDate = Sic.Web.Mvc.Session.CurrentDateTime.Date;
                        medicalCare.MedicalCareMedications = new List<MedicalCareMedication>();
                        for (short i = 1; i <= 10; i++)
                            medicalCare.MedicalCareMedications.Add(new MedicalCareMedication() { Priority = i });
                    }
                    else
                    {
                        medicalCare = DataBase.MedicalCares.Get(p =>
                            p.PatientId == patientId &&
                            p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();

                        for (short i = 1; i <= 10; i++)
                        {
                            if (!medicalCare.MedicalCareMedications.Any(p => p.Priority == i))
                                medicalCare.MedicalCareMedications.Add(new MedicalCareMedication() { Priority = i });
                        }
                    }
                    return PartialView("_EditMedicalCare", medicalCare);
                }
                catch
                {
                    this.AddDefaultErrorMessage();
                }
            }
            
            return Json();
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult Delete(int medicalCareId, int patientId)
        {
            if (IsValidProfesionalPatient(patientId))
            {
                try
                {
                    MedicalCare medicalCare = DataBase.MedicalCares.Get(p => 
                        p.PatientId == patientId &&
                        p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                    
                    DataBase.MedicalCares.Delete(medicalCareId);
                    
                    foreach (var medication in medicalCare.MedicalCareMedications)
                        DataBase.MedicalCareMedications.Delete(medication);

                    DataBase.Save();
                    
                    this.AddDefaultSuccessMessage();

                }
                catch
                {
                    this.AddDefaultErrorMessage();
                }
            }
            return Json();
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]        
        public JsonResult Update(Models.Medical.View.MedicalCareEdit medicalCare)
        {
            try
            {
                if (!this.IsValidProfesionalPatient(medicalCare.PatientId, true)) return Json();                

                bool insert = false;
                if (medicalCare.MedicalCareId == 0) insert = true;

                MedicalCare medicalCareUpdate = null;

                if (insert)
                {
                    medicalCareUpdate = new Models.Medical.MedicalCare();
                    medicalCareUpdate.RecordDate = Sic.Web.Mvc.Session.CurrentDateTime;
                }
                else
                {
                    medicalCareUpdate = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalCare.MedicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                    medicalCareUpdate.UpdateRecordDate = Sic.Web.Mvc.Session.CurrentDateTime;
                }

                medicalCareUpdate.ProfessionalId = this.ProfessionalId;
                medicalCareUpdate.Evolution = medicalCare.Evolution;
                medicalCareUpdate.Diagnostic = medicalCare.Diagnostic;
                medicalCareUpdate.Treatment = medicalCare.Treatment;
                medicalCareUpdate.PatientId = medicalCare.PatientId;
                medicalCareUpdate.EvolutionDate = medicalCare.EvolutionDate;               

                if (string.IsNullOrWhiteSpace(medicalCareUpdate.Treatment)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Evolution)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Diagnostic)
                    && (medicalCare.MedicalCareMedications == null || !medicalCare.MedicalCareMedications.Any()))
                {
                   this.AddErrorMessage(Sic.Apollo.Resources.MessageFor.VerifyIncorrectData);
                }

                List<MedicalCareMedication> medicationsUpdate = new List<MedicalCareMedication>();
                List<MedicalCareMedication> medications = new List<MedicalCareMedication>();
                if (!insert)
                    medicationsUpdate.AddRange(medicalCareUpdate.MedicalCareMedications);

               
                foreach (MedicalCareMedicationEdit edit in medicalCare.Medications)
                {
                    //long professionalVademecumId = Convert.ToInt64(row["pvid"]);
                    //string name = Convert.ToString(row["name"]);
                    //string posology = Convert.ToString(row["pos"]);
                    //long vademecumId = Convert.ToInt64(row["gid"]);
                    //long medicalCareMedicationId = Convert.ToInt64(row["id"]);

                    MedicalCareMedication medication;

                    if (insert)
                        medication = new MedicalCareMedication();
                    else
                    {
                        medication = medicationsUpdate.SingleOrDefault(p => p.MedicalCareMedicationId == edit.MedicalCareMedicationId);
                        if (medication == null)
                            medication = new MedicalCareMedication();
                    }

                    if (edit.ProfessionalVademecumId == 0)
                    {
                        ProfessionalVademecum pVademecum = null;
                        pVademecum = DataBase.ProfessionalVademecums.Get(p => p.Name == edit.MedicationName).FirstOrDefault();
                        if (pVademecum == null)
                        {
                            pVademecum = new ProfessionalVademecum();

                            pVademecum.Name = edit.MedicationName;
                            pVademecum.Posology = edit.Posology;
                            pVademecum.ProfessionalId = this.ProfessionalId;
                            pVademecum.Active = true;
                            if (edit.GeneralVademecumId != 0)
                                pVademecum.VademecumId = edit.GeneralVademecumId;

                            DataBase.ProfessionalVademecums.Insert(pVademecum);
                        }
                        medication.ProfessionalVademecum = pVademecum;
                    }
                    else
                        medication.ProfessionalVademecumId = edit.ProfessionalVademecumId;

                    medication.MedicationName = edit.MedicationName;
                    medication.Posology = edit.Posology;
                    medication.Priority = edit.Priority;
                    medication.Quantity = edit.Quantity;

                    if (insert || medication.MedicalCareMedicationId == 0)
                    {
                        medication.MedicalCare = medicalCareUpdate;
                        DataBase.MedicalCareMedications.Insert(medication);
                    }
                    else
                        DataBase.MedicalCareMedications.Update(medication);

                    medications.Add(medication);
                }
                

                //Delete Medications
                if (!insert)
                {
                    foreach (var delete in medicationsUpdate.Where(p => !medications.Any(q => q.MedicalCareMedicationId == p.MedicalCareMedicationId)))
                        DataBase.MedicalCareMedications.Delete(delete);
                }

                if (insert)
                    DataBase.MedicalCares.Insert(medicalCareUpdate);
                else
                    DataBase.MedicalCares.Update(medicalCareUpdate);

                DataBase.Save();

                this.AddDefaultSuccessMessage();
                
                return Json(new { MedicalCareId = medicalCareUpdate.MedicalCareId });
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult VademecumsAutocomplete(string term)
        {
            var vademecums = DataBase.Professionals.FindVademecums(term, Sic.Apollo.Session.ProfessionalId);

            var result = vademecums.Select(p => new { label = p.Name, gid = p.VademecumId, id = p.ProfessionalVademecumId, pos = (p.Posology ?? string.Empty) }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public void PrintPatientMedication(long medicalCareId, int contactLocationId)
        {
            var medicalCare = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalCareId,
                includeProperties: "MedicalCareMedications,Professional.Contact,Professional.ProfessionalOffices,Patient.Contact").SingleOrDefault();

            var medications = DataBase.MedicalCareMedications.Get(p => p.MedicalCareId == medicalCareId);
            var professional = medicalCare.Professional;
            ProfessionalOffice office = null;
            if (contactLocationId == 0)
                office = professional.ProfessionalOffices.FirstOrDefault();
            else
                office = professional.ProfessionalOffices.SingleOrDefault(p => p.ContactLocationId == contactLocationId);
            var patient = medicalCare.Patient;

            WebFormReportSettings report = new WebFormReportSettings();
            report.ReportPath = Server.MapPath("~/Reports/PatientMedication.rdlc");

            report.AddDataSource("Medication", medications);

            report.AddDataSource("Professional", professional);

            report.AddDataSource("ProfessionalOffice", office );

            report.AddDataSource("Patient", patient );

            RenderReport(report);                       
        }        
	}
}