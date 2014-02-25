using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Web.Mvc;
using System.IO;
using System.Data;
using Sic.Apollo.Models.Pro;
using Microsoft.Reporting.WebForms;
using Sic.Apollo.Reports.DataSource;

namespace Sic.Apollo.Controllers
{
    public class PatientController : Sic.Web.Mvc.Controllers.BaseController
    {
        private const int maxSizeUploadFile = 5242880;//5 MB;
        private const string FilePath = "~/Content/db/patients/{0}";
        private const string FileSavePath = "/Content/db/patients/{0/}";

        //
        // GET: /Patient/
        ContextService db = new ContextService();

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Create()
        {
            return PartialView(new Models.Medical.View.Patient());
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult List()
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Patients;
            byte active = (byte)PatientState.Active;
            var patiens = db.ProfessionalPatients.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId && p.State == active);
            return View(patiens);
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ActionName("Epicrisis")]
        public ActionResult Profile(Models.Medical.Patient patient, int patientId)
        {
            ViewBag.ProfileDefault = true;
            Models.Medical.Patient patientUpdate = null;            
            try
            {
                patientUpdate = db.Patients.Get(p => p.PatientId == patient.PatientId,
                        includeProperties: "Contact,PatientInsuranceInstitutions").Single();

                var insuranceList = patientUpdate.PatientInsuranceInstitutions.ToList();

                TryUpdateModel(patientUpdate, null, null, new string[] { "PatientInsuranceInstitutions" });

                 

                patientUpdate.Validate(ModelState);

                //News
                foreach (var insurance in patient.PatientInsuranceInstitutions.Where(p =>
                    p.PatientInsuranceInstitutionId == 0 && p.InsuranceInstitutionId != 0))
                {
                    db.PatientInsuranceInstitutions.Insert(insurance);
                    insurance.PatientId = patientUpdate.PatientId;
                    patientUpdate.PatientInsuranceInstitutions.Add(insurance);
                }

                //Update
                foreach (var insurance in patient.PatientInsuranceInstitutions.Where(p => p.PatientInsuranceInstitutionId != 0))
                {
                    var insuranceUpdate = patientUpdate.PatientInsuranceInstitutions.SingleOrDefault(p => p.Priority == insurance.Priority);

                    if (insuranceUpdate != null)
                    {
                        if (insurance.InsuranceInstitutionId != 0)
                        {
                            insuranceUpdate.InsuranceInstitution = db.InsuranceInstitutions.GetByID(insurance.InsuranceInstitutionId);
                            insuranceUpdate.InsuranceInstitutionId = insuranceUpdate.InsuranceInstitution.InstitutionId;
                            insuranceUpdate.InsuranceInstitutionPlanName = insurance.InsuranceInstitutionPlanName;
                            insuranceUpdate.RegistrationCode = insurance.RegistrationCode;
                        }
                        else
                        {
                            db.PatientInsuranceInstitutions.Delete(insuranceUpdate);
                            patientUpdate.PatientInsuranceInstitutions.Remove(insuranceUpdate);
                        }
                    }
                }

                db.Contacts.Update(patientUpdate.Contact); 
                db.Patients.Update(patientUpdate);
                db.Save();                
                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
                ViewBag.MessageType = Sic.Constants.MessageType.Success;
            }
            catch (Exception)
            {                
                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                ViewBag.MessageType = Sic.Constants.MessageType.Error;                
            }            

            var returnPatient = GetPatient(patient.PatientId);
            PrepareEpicrisis(returnPatient.Patient);
            return View("Epicrisis",returnPatient);
            //return RedirectToAction("Epicrisis", new { patientId = patientUpdate.PatientId, op = success });
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult MedicalCare(long medicalHistoryId)
        {
            var history = db.MedicalCares.Get(p => p.MedicalCareId == medicalHistoryId, includeProperties: "MedicalCareMedications").SingleOrDefault();
            return PartialView(history);
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult MedicationPrint(int patientId)
        {
            Professional pro = db.Professionals.Get(p=>p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalOffices,ProfessionalOffices.ContactLocation.City,ProfessionalSpecializations.Specialization").SingleOrDefault();
                      
            return View(pro);
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
                medicalCare = db.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
                for (short i = 1; i <= 10; i++)
                {
                    if(!medicalCare.MedicalCareMedications.Any(p=>p.Priority==i))
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
                MedicalCare medicalCare = db.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();                
                db.MedicalCares.Delete(medicalCareId);
                foreach(var medication in medicalCare.MedicalCareMedications)
                    db.MedicalCareMedications.Delete(medication);
                db.Save();
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
                    medicalCareUpdate = db.MedicalCares.Get(p => p.MedicalCareId == history.MedicalCareId, includeProperties: "MedicalCareMedications").SingleOrDefault();
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

                if(string.IsNullOrWhiteSpace(medicalCareUpdate.Treatment)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Evolution)
                    && string.IsNullOrWhiteSpace(medicalCareUpdate.Diagnostic)
                    && !(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0))
                {
                    return new JsonResult()
                    {
                        Data = new {
                            MessageType = Sic.Constants.MessageType.Error, 
                            Success = false, Message = Sic.Apollo.Resources.Resources.MessageForVerifyIncorrectData }
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
                            pVademecum = db.ProfessionalVademecums.Get(p => p.Name == name).FirstOrDefault();
                            if (pVademecum == null)
                            {
                                pVademecum = new ProfessionalVademecum();

                                pVademecum.Name = name;
                                pVademecum.Posology = posology;
                                pVademecum.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                                pVademecum.Active = true;
                                if (vademecumId != 0)
                                    pVademecum.VademecumId = vademecumId;

                                db.ProfessionalVademecums.Insert(pVademecum);
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
                            db.MedicalCareMedications.Insert(medication);
                        }
                        else
                            db.MedicalCareMedications.Update(medication);

                        medications.Add(medication);
                    }
                }

                //Delete Medications
                if (!insert)
                {
                    foreach (var delete in medicationsUpdate.Where(p=> !medications.Any(q=>q.MedicalCareMedicationId == p.MedicalCareMedicationId) ))
                        db.MedicalCareMedications.Delete(delete);
                }

                if(insert)
                    db.MedicalCares.Insert(medicalCareUpdate);
                else
                    db.MedicalCares.Update(medicalCareUpdate);

                db.Save();
                
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
                    Data = new { Success = false, 
                        MessageType = Sic.Constants.MessageType.Error, 
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure }
                };
            }
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public JsonResult SaveMedicalProblem(int patientId, string medicalProblems)
        {
            try
            {
                System.IO.StringReader sr = new System.IO.StringReader(medicalProblems);
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                ds.ReadXml(sr);

                List<MedicalHistory> medicalProblemValues = new List<MedicalHistory>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    medicalProblemValues.Add(new MedicalHistory
                    {
                        MedicalHistoryId = Convert.ToInt32(row["pid"]),
                        MedicalProblemId = Convert.ToInt16(row["mdpi"]),
                        PatientId = patientId,
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        Description = Convert.ToString(row["pd"])
                    });
                }

                DateTime currentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;
                foreach (var medicalProblem in medicalProblemValues)
                {
                    if (medicalProblem.MedicalHistoryId == 0 && !string.IsNullOrWhiteSpace(medicalProblem.Description))
                    {
                        medicalProblem.RecordDate = currentDateTime;
                        db.MedicalHistories.Insert(medicalProblem);
                    }
                    else if (medicalProblem.MedicalHistoryId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(medicalProblem.Description))
                        {
                            db.MedicalHistories.Update(medicalProblem);
                            medicalProblem.RecordDate = currentDateTime;
                        }
                        else
                            db.MedicalHistories.Delete(medicalProblem);
                    }
                }

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        MessageType = Sic.Constants.MessageType.Success
                    }
                };
            }
            catch (Exception)
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error }
                };
            }
        }
        
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public ActionResult VitalSignHistory(int patientId)
        {
            return PartialView(db.PatientVitalSigns.Get(p=> p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                && p.PatientId == patientId, includeProperties : "VitalSign"));
        }


        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public JsonResult SaveVitalSign(int patientId,string vitalSigns, long dateTicks)
        {
            try
            {
                DateTime date = new DateTime(dateTicks);

                //var patient = db.Patients.Get(p=>p.PatientId == patientId, includeProperties:"PatientVitalSigns").SingleOrDefault();
                System.IO.StringReader sr = new System.IO.StringReader(vitalSigns);
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                ds.ReadXml(sr);

                List<PatientVitalSign> vitalSignValues = new List<PatientVitalSign>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    vitalSignValues.Add(new PatientVitalSign() 
                    { 
                        VitalSignId = Convert.ToInt32(row["vid"]),
                        MeasuringUnit = Convert.ToInt16(row["munit"]),
                        PatientId = patientId,
                        PatientVitalSignId = Convert.ToInt64(row["id"]),
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        Value = row["val"].ToString()
                    });                 
                }

                DateTime currentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;
                DateTime currentDate = date;

                List<PatientVitalSign> currentVitalSigns = db.PatientVitalSigns.Get(p => p.PatientId == patientId &&
                    p.RecordDate == currentDate).ToList();
                
                foreach (var patientVitalSign in vitalSignValues)
                {                    
                    PatientVitalSign patientVitalSignUpdate = currentVitalSigns.FirstOrDefault(p => p.VitalSignId == patientVitalSign.VitalSignId);

                    if (patientVitalSignUpdate == null)
                        patientVitalSignUpdate = patientVitalSign;
                    else
                        patientVitalSignUpdate.Value = patientVitalSign.Value;

                    if (patientVitalSignUpdate.PatientVitalSignId == 0 && !string.IsNullOrWhiteSpace(patientVitalSignUpdate.Value))
                    {
                        patientVitalSignUpdate.VitalSignDate = date;
                        patientVitalSignUpdate.RecordDate = currentDateTime;
                        db.PatientVitalSigns.Insert(patientVitalSignUpdate);
                    }
                    else if (patientVitalSignUpdate.PatientVitalSignId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(patientVitalSignUpdate.Value))
                        {
                            db.PatientVitalSigns.Update(patientVitalSignUpdate);
                            patientVitalSignUpdate.VitalSignDate = date;
                            patientVitalSignUpdate.RecordDate = currentDateTime;
                        }
                        else
                            db.PatientVitalSigns.Delete(patientVitalSignUpdate);
                    }
                }

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,                       
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        MessageType = Sic.Constants.MessageType.Success
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error }
                };
            }
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public JsonResult SavePhysicalExamination(int patientId, string physicalExaminatons)
        {
            try
            {                
                System.IO.StringReader sr = new System.IO.StringReader(physicalExaminatons);
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                ds.ReadXml(sr);

                List<PatientPhysicalExamination> patientPhysicalExaminations = new List<PatientPhysicalExamination>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    patientPhysicalExaminations.Add(new PatientPhysicalExamination
                    {
                        PatientPhysicalExaminationId = Convert.ToInt32(row["id"]),                        
                        PatientId = patientId,
                        PhysicalExaminationId = Convert.ToInt32(row["peid"]),
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        Examination = Convert.ToString(row["val"])
                    });
                }

                DateTime currentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;
                DateTime currentDate = currentDateTime.Date;

                List<PatientPhysicalExamination> currentPhysicalExaminations = db.PatientPhysicalExaminations.Get(p => p.PatientId == patientId &&
                    p.RecordDate >= currentDate).ToList();

                foreach (var patientPhysicalExamination in patientPhysicalExaminations)
                {
                    PatientPhysicalExamination patientPhysicalExaminationUpdate = currentPhysicalExaminations.FirstOrDefault(p => p.PhysicalExaminationId == patientPhysicalExamination.PhysicalExaminationId);

                    if (patientPhysicalExaminationUpdate == null)
                        patientPhysicalExaminationUpdate = patientPhysicalExamination;
                    else
                        patientPhysicalExaminationUpdate.Examination = patientPhysicalExamination.Examination;

                    if (patientPhysicalExaminationUpdate.PatientPhysicalExaminationId == 0 &&
                        !string.IsNullOrWhiteSpace(patientPhysicalExaminationUpdate.Examination))
                    {
                        patientPhysicalExaminationUpdate.RecordDate = currentDateTime;
                        db.PatientPhysicalExaminations.Insert(patientPhysicalExaminationUpdate);                        
                    }
                    else if (patientPhysicalExaminationUpdate.PatientPhysicalExaminationId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(patientPhysicalExaminationUpdate.Examination))
                        {
                            db.PatientPhysicalExaminations.Update(patientPhysicalExaminationUpdate);
                            patientPhysicalExaminationUpdate.RecordDate = currentDateTime;                            
                        }
                        else
                            db.PatientPhysicalExaminations.Delete(patientPhysicalExaminationUpdate);
                    }
                }

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        MessageType = Sic.Constants.MessageType.Success
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error }
                };
            }
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeletePatient(int professionalPatientId)
        {
            try{

                ProfessionalPatient pPatient = db.ProfessionalPatients.GetByID(professionalPatientId);
                pPatient.State = (byte)PatientState.Inactive;
                db.ProfessionalPatients.Update(pPatient);
                db.Save();

                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForDeletePatientSuccess;
                ViewBag.MessageType = Sic.Constants.MessageType.Success;
            }
            catch
            {
                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                ViewBag.MessageType = Sic.Constants.MessageType.Error;
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult CreatePatient(Models.Medical.View.Patient patient)
        {
            try
            {
                int professionalId = Sic.Apollo.Session.ProfessionalId;
                Models.Medical.Patient patientInsert = new Patient();
                Models.General.Contact contact = new Models.General.Contact()
                {
                    FirstName = patient.FirstName,
                    MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    SecondLastName = patient.SecondLastName,
                    Gender = patient.Gender,
                    PhoneNumber = patient.PhoneNumber,
                    CellPhone = patient.CellPhone
                };
                patientInsert.Contact = contact;

                //ProfessionalPatient professionalPatient = new ProfessionalPatient();
                //professionalPatient.
                ProfessionalPatient professionalPatient = new ProfessionalPatient();
                professionalPatient.Patient = patientInsert;
                professionalPatient.ProfessionalId = professionalId;
                professionalPatient.State = 1;

                db.Contacts.Insert(patientInsert.Contact);
                db.Patients.Insert(patientInsert);
                db.ProfessionalPatients.Insert(professionalPatient);

                Models.Pro.Customer customer = new Models.Pro.Customer();
                customer.Contact = patientInsert.Contact;

                db.Customers.Insert(customer);

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        PatientId = patientInsert.PatientId,
                        Success = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        PatientCount = db.Professionals.GetPatientCount(professionalId)
                    }
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

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult FileUpload(HttpPostedFileWrapper file, int patientId)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedFailure,
                            ImagePath = string.Empty
                        }
                    };
                }

                if (file.ContentLength > maxSizeUploadFile)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = String.Format(Sic.Apollo.Resources.Resources.MessageForFileMaxSizeValidation, (maxSizeUploadFile / 1048576)),
                            ImagePath = string.Empty
                        }
                    };
                }

                var fileName = String.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientId))), fileName);
                var folder = Path.GetDirectoryName(imagePath);
                bool isExists = System.IO.Directory.Exists(folder);
                if (!isExists)
                    System.IO.Directory.CreateDirectory(folder);

                file.SaveAs(imagePath);
                if(Sic.Web.Mvc.Utility.MimeType.IsImage(Path.GetExtension(imagePath)))
                    Sic.Web.Mvc.Utility.Thumbnail.SaveThumbnail(imagePath);

                PatientFile patientFile = new PatientFile();
                patientFile.Name = Path.GetFileNameWithoutExtension(file.FileName);
                patientFile.PatientFileName = fileName;
                patientFile.PatientId = patientId;
                patientFile.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                patientFile.UploadDate = Sic.Web.Mvc.Session.CurrentDateTime;
                patientFile.MimeType = Sic.Web.Mvc.Utility.MimeType.GetMimeType(Path.GetExtension(patientFile.PatientFileName));
                db.PatientFiles.Insert(patientFile);

                db.Save();

                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForFileUploadedSuccess,
                        PatientFileId = patientFile.PatientFileId
                    }
                };
            }
            catch
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForFileUploadedFailure
                    }
                };
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        [HttpPost]
        public ActionResult DeletePatientFile(int patientFileId)
        {
            try
            {
                PatientFile patientFile = db.PatientFiles.GetByID(patientFileId);

                db.PatientFiles.Delete(patientFile);
                db.Save();

                var imagePath = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientFile.PatientId))), patientFile.PatientFileName);
                FileInfo FileDetele = new FileInfo(imagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                if (patientFile.IsImageType)
                {
                    string nameMin = Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(patientFile.PatientFileName);
                    if(nameMin.StartsWith("\\")){
                        nameMin = nameMin.Substring(1);
                    }
                    var imagePathMini = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientFile.PatientId))), 
                        nameMin);
                    FileInfo FileDeteleMin = new FileInfo(imagePathMini);
                    if (FileDeteleMin.Exists)
                        FileDeteleMin.Delete();
                }

                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk
                    }
                };
            }
            catch
            {
                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction()]
        [HttpPost]
        public ActionResult EditPatientFile(int patientFileId, string name, string comment = null)
        {
            try
            {
                PatientFile patientFile = db.PatientFiles.GetByID(patientFileId);
                patientFile.Comment = comment;
                patientFile.Name = name;

                db.PatientFiles.Update(patientFile);
                db.Save();

                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk
                    }
                };
            }
            catch
            {
                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }

        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]        
        public ActionResult PatientFile(int patientFileId)
        {
            PatientFile patientFile = db.PatientFiles.GetByID(patientFileId);

            return PartialView("_PatientFile",patientFile);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Epicrisis(int patientId)
        {
            var patient = GetPatient(patientId);
            if(patient==null)
                return RedirectToAction("ResourceNotFound", "Error");

            PrepareEpicrisis(patient.Patient);           
            return View(patient);
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Resume(int patientId)
        {
            var patient = GetPatient(patientId);
            if (patient == null)
                return RedirectToAction("ResourceNotFound", "Error");
            PrepareEpicrisis(patient.Patient);           
            return PartialView(patient.Patient);
        }        

        private ProfessionalPatient GetPatient(int patientId)
        {
            byte active = (byte)PatientState.Active;
            return db.ProfessionalPatients.Get(p => p.PatientId == patientId && 
                p.ProfessionalId == Sic.Apollo.Session.ProfessionalId && p.State == active, 
                includeProperties: "Patient.Contact,Patient.PatientInsuranceInstitutions,Patient.PatientVitalSigns,Patient.PatientFiles").SingleOrDefault();
        }

        private void PrepareEpicrisis(Models.Medical.Patient patient)
        {
            DateTime currentDate = Sic.Web.Mvc.Session.CurrentDateTime;
            ViewBag.ProfessionalOption = ProfessionalOption.Epicrisis;
            ViewBag.PatientId = patient.PatientId;

            Professional pro = db.Professionals.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalOffices.City,ProfessionalSpecializations.Specialization").SingleOrDefault();

            /*For Medical Print*/
            ViewBag.Professional = pro;
            ViewBag.PatientName = patient.Contact.FullName;

            #region Insurance

            int insuranceAllow = 2;
            var insurances = db.InsuranceInstitutions.Get(p => p.Active);

            for (byte i = 1; i <= insuranceAllow; i++)
            {
                if (!patient.PatientInsuranceInstitutions.Any(p => p.Priority == i))
                {
                    patient.PatientInsuranceInstitutions.Add(new PatientInsuranceInstitution() { Priority = i });
                }
            }
            
            ViewBag.InsuranceInstitutions = insurances;

            #endregion

            patient.ResumeMedicalCares.AddRange(patient.MedicalCares.OrderByDescending(p=>p.RecordDate).Take(3));            

            #region VitalSign

            var vitalSigns = db.VitalSigns.Get(p => p.Active).OrderBy(p => p.Priority);
            var currentVitalSigns = patient.PatientVitalSigns.Where(p => p.VitalSignDate.Date == currentDate.Date).ToList();
            
            foreach (var vitalSign in vitalSigns)
            {
                var lastVitalSign = patient.PatientVitalSigns.Where(p => p.VitalSignId == vitalSign.VitalSignId).
                    OrderByDescending(p=>p.VitalSignDate).FirstOrDefault();

                if (lastVitalSign != null)
                    patient.ResumePatientVitalSigns.Add(lastVitalSign);

                if (!currentVitalSigns.Any(p => p.VitalSignId == vitalSign.VitalSignId))
                    patient.PatientVitalSigns.Add(new PatientVitalSign()
                    {
                        VitalSignId = vitalSign.VitalSignId,
                        PatientId = patient.PatientId,
                        VitalSign = vitalSign,
                        MeasuringUnit = vitalSign.DefaultMeasurementUnit,                        
                        VitalSignDate = currentDate,
                        RecordDate = currentDate
                    });
            }

            #endregion

            #region Physical Examination

            var physicalExaminations = db.PhysicalExaminations.Get(p => p.Active).OrderBy(p => p.Priority);
            var currentPhysicalExamination = patient.PatientPhysicalExaminations.Where(p =>p.RecordDate.Date == currentDate.Date);

            foreach (var physicalExamination in physicalExaminations)
            {
                var lastPhysicalExamination = patient.PatientPhysicalExaminations.Where(p => p.PhysicalExaminationId == physicalExamination.PhysicalExaminationId).
                    OrderByDescending(p => p.RecordDate).FirstOrDefault();

                if (lastPhysicalExamination != null)
                    patient.ResumePatientPhysicalExamination.Add(lastPhysicalExamination);

                if (!currentPhysicalExamination.Any(p => p.PhysicalExaminationId == physicalExamination.PhysicalExaminationId))
                    patient.PatientPhysicalExaminations.Add(new PatientPhysicalExamination()
                    {
                        PhysicalExaminationId = physicalExamination.PhysicalExaminationId,
                        PatientId = patient.PatientId,
                        PhysicalExamination = physicalExamination,                        
                        RecordDate = currentDate
                    });
            }

            #endregion

            #region MedicalHistory

            var medicalProblems = db.MedicalProblems.Get(p => p.Active).OrderBy(P=>P.Priority);
            patient.ResumeMedicalHistories.AddRange(patient.MedicalHistories);

            foreach (var problem in medicalProblems.Where(p=>!patient.MedicalHistories.Any(q=>q.MedicalProblemId == p.MedicalProblemId)))
            {
                patient.MedicalHistories.Add(new MedicalHistory()
                {
                    MedicalProblem = problem,
                    MedicalProblemId = problem.MedicalProblemId,
                    Patient = patient,
                    PatientId = patient.PatientId,
                    ProfessionalId = Sic.Apollo.Session.ProfessionalId                    
                });
            }

            #endregion
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult PatientsAutocomplete(string term)
        {
            var patiens = db.Patients.GetPatientPerson(term, Sic.Apollo.Session.ProfessionalId);

            var result = patiens.Select(p => new { label = p.FullName, id = p.ContactId }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult PrintPatientMedication(long medicalCareId, int contactLocationId)
        {
            var medicalCare = db.MedicalCares.Get(p => p.MedicalCareId == medicalCareId, 
                includeProperties: "MedicalCareMedications,Professional.Contact,Professional.ProfessionalOffices,Patient.Contact").SingleOrDefault();
            
            var medications = db.MedicalCareMedications.Get(p => p.MedicalCareId == medicalCareId);
            var professional = medicalCare.Professional;
            ProfessionalOffice office = null;
            if (contactLocationId == 0)
                office = professional.ProfessionalOffices.FirstOrDefault();
            else
                office = professional.ProfessionalOffices.SingleOrDefault(p => p.ContactLocationId == contactLocationId);
            var patient = medicalCare.Patient;

            var ReportPath = Server.MapPath("~/Reports/PatientMedication.rdlc");

            ReportDataSource dsMedication = new ReportDataSource("Medication",medications);

            ReportDataSource dsProfessional = new ReportDataSource("Professional",
                new List<Professional> { professional });

            ReportDataSource dsProfessionalOffice = new ReportDataSource("ProfessionalOffice",
                new List<ProfessionalOffice> { office });

            ReportDataSource dsPatient = new ReportDataSource("Patient",
                new List<Patient> { patient });

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

