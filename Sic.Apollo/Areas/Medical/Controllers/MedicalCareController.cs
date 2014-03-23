using Microsoft.Reporting.WebForms;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Apollo.Models.Pro;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Medical.Controllers
{
    public class MedicalCareController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult MedicalCare(long medicalHistoryId)
        {
            var history = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalHistoryId, includeProperties: "MedicalCareMedications").SingleOrDefault();
            return PartialView(history);
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditMedicalCare(long medicalCareId)
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
                medicalCare = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                for (short i = 1; i <= 10; i++)
                {
                    if (!medicalCare.MedicalCareMedications.Any(p => p.Priority == i))
                        medicalCare.MedicalCareMedications.Add(new MedicalCareMedication() { Priority = i });
                }
            }
            return PartialView("EditMedicalCare", medicalCare);
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult DeleteMedicalCare(int medicalCareId)
        {
            try
            {
                MedicalCare medicalCare = DataBase.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                DataBase.MedicalCares.Delete(medicalCareId);
                foreach (var medication in medicalCare.MedicalCareMedications)
                    DataBase.MedicalCareMedications.Delete(medication);
                
                DataBase.Save();
                return new JsonResult()
                {
                    Data = new { Success = true, Message = Sic.Apollo.Resources.Resources.MessageForSaveOk }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure }
                };
            }
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public JsonResult UpdateMedicalCare(Models.Medical.MedicalCare history)
        {
            try
            {
                bool insert = false;
                if (history.MedicalCareId == 0) insert = true;

                MedicalCare medicalCareUpdate = null;

                if (insert)
                {
                    medicalCareUpdate = new Models.Medical.MedicalCare();
                    medicalCareUpdate.RecordDate = Sic.Web.Mvc.Session.CurrentDateTime;
                }
                else
                {
                    medicalCareUpdate = DataBase.MedicalCares.Get(p => p.MedicalCareId == history.MedicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                    medicalCareUpdate.UpdateRecordDate = Sic.Web.Mvc.Session.CurrentDateTime;
                }

                medicalCareUpdate.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                medicalCareUpdate.Evolution = history.Evolution;
                medicalCareUpdate.Diagnostic = history.Diagnostic;
                medicalCareUpdate.Treatment = history.Treatment;
                medicalCareUpdate.PatientId = history.PatientId;
                medicalCareUpdate.EvolutionDate = history.EvolutionDate;

                System.IO.StringReader sr = new System.IO.StringReader(history.MedicationsClientXml);
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                ds.ReadXml(sr);

                if (string.IsNullOrWhiteSpace(medicalCareUpdate.Treatment)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Evolution)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Diagnostic)
                    && !(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0))
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            MessageType = Sic.Constants.MessageType.Error,
                            Success = false,
                            Message = Sic.Apollo.Resources.Resources.MessageForVerifyIncorrectData
                        }
                    };
                }

                List<MedicalCareMedication> medicationsUpdate = new List<MedicalCareMedication>();
                List<MedicalCareMedication> medications = new List<MedicalCareMedication>();
                if (!insert)
                    medicationsUpdate.AddRange(medicalCareUpdate.MedicalCareMedications);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        long professionalVademecumId = Convert.ToInt64(row["pvid"]);
                        string name = Convert.ToString(row["name"]);
                        string posology = Convert.ToString(row["pos"]);
                        long vademecumId = Convert.ToInt64(row["gid"]);
                        long medicalCareMedicationId = Convert.ToInt64(row["id"]);

                        MedicalCareMedication medication = null;
                        if (insert)
                            medication = new MedicalCareMedication();
                        else
                        {
                            medication = medicationsUpdate.SingleOrDefault(p => p.MedicalCareMedicationId == medicalCareMedicationId);
                            if (medication == null)
                                medication = new MedicalCareMedication();
                        }

                        if (professionalVademecumId == 0)
                        {
                            ProfessionalVademecum pVademecum = null;
                            pVademecum = DataBase.ProfessionalVademecums.Get(p => p.Name == name).FirstOrDefault();
                            if (pVademecum == null)
                            {
                                pVademecum = new ProfessionalVademecum();

                                pVademecum.Name = name;
                                pVademecum.Posology = posology;
                                pVademecum.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                                pVademecum.Active = true;
                                if (vademecumId != 0)
                                    pVademecum.VademecumId = vademecumId;

                                DataBase.ProfessionalVademecums.Insert(pVademecum);
                            }
                            medication.ProfessionalVademecum = pVademecum;
                        }
                        else
                            medication.ProfessionalVademecumId = professionalVademecumId;

                        medication.MedicationName = name;
                        medication.Posology = posology;
                        medication.Priority = Convert.ToByte(row["pr"]);
                        medication.Quantity = Convert.ToByte(row["q"]);

                        if (insert || medication.MedicalCareMedicationId == 0)
                        {
                            medication.MedicalCare = medicalCareUpdate;
                            DataBase.MedicalCareMedications.Insert(medication);
                        }
                        else
                            DataBase.MedicalCareMedications.Update(medication);

                        medications.Add(medication);
                    }
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

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        MessageType = Sic.Constants.MessageType.Success,
                        MedicalHistoryId = medicalCareUpdate.MedicalCareId,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        MessageType = Sic.Constants.MessageType.Error,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult PrintPatientMedication(long medicalCareId, int contactLocationId)
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

            var ReportPath = Server.MapPath("~/Reports/PatientMedication.rdlc");

            ReportDataSource dsMedication = new ReportDataSource("Medication", medications);

            ReportDataSource dsProfessional = new ReportDataSource("Professional",
                new List<Professional> { professional });

            ReportDataSource dsProfessionalOffice = new ReportDataSource("ProfessionalOffice",
                new List<ProfessionalOffice> { office });

            ReportDataSource dsPatient = new ReportDataSource("Patient",
                new List<Models.Medical.Patient> { patient });

            RenderReport(ReportPath, new List<ReportDataSource> { dsMedication, dsProfessional, 
                dsProfessionalOffice, dsPatient });

            return View();
        }

        private void RenderReport(string ReportPath, List<ReportDataSource> reportsDataSource)
        {

            var localReport = new LocalReport { ReportPath = ReportPath };

            //Give the collection a name (EmployeeCollection) so that we can reference it in our report designer            
            foreach (ReportDataSource ds in reportsDataSource)
                localReport.DataSources.Add(ds);

            var reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            var deviceInfo =
                //string.Format("<DeviceInfo><OutputFormat>{0}</OutputFormat><PageWidth>8.5in</PageWidth><PageHeight>11in</PageHeight><MarginTop>0.5in</MarginTop><MarginLeft>1in</MarginLeft><MarginRight>1in</MarginRight><MarginBottom>0.5in</MarginBottom></DeviceInfo>", reportType);
                string.Format("<DeviceInfo><OutputFormat>{0}</OutputFormat></DeviceInfo>", reportType);

            Warning[] warnings;
            string[] streams;

            //Render the report
            var renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            //Clear the response stream and write the bytes to the outputstream
            //Set content-disposition to "attachment" so that user is prompted to take an action
            //on the file (open or save)
            Response.Clear();
            Response.ContentType = mimeType;
            //Response.AddHeader("content-disposition", "attachment; filename=foo." + fileNameExtension);
            Response.AddHeader("content-disposition", "inline; filename=foo." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }
	}
}