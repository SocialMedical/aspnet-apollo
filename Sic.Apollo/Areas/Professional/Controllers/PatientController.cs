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

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class PatientController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {                
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Create()
        {
            return PartialView("_Create", new Models.Medical.View.Patient());
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult List()
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Patients;
            byte active = (byte)PatientState.Active;
            var patiens = DataBase.ProfessionalPatients.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId && p.State == active);
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
                patientUpdate = DataBase.Patients.Get(p => p.PatientId == patient.PatientId,
                        includeProperties: "Contact,PatientInsuranceInstitutions").Single();

                var insuranceList = patientUpdate.PatientInsuranceInstitutions.ToList();

                TryUpdateModel(patientUpdate, null, null, new string[] { "PatientInsuranceInstitutions" });

                 

                patientUpdate.Validate(ModelState);

                //News
                foreach (var insurance in patient.PatientInsuranceInstitutions.Where(p =>
                    p.PatientInsuranceInstitutionId == 0 && p.InsuranceInstitutionId != 0))
                {
                    DataBase.PatientInsuranceInstitutions.Insert(insurance);
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
                            insuranceUpdate.InsuranceInstitution = DataBase.InsuranceInstitutions.GetByID(insurance.InsuranceInstitutionId);
                            insuranceUpdate.InsuranceInstitutionId = insuranceUpdate.InsuranceInstitution.InstitutionId;
                            insuranceUpdate.InsuranceInstitutionPlanName = insurance.InsuranceInstitutionPlanName;
                            insuranceUpdate.RegistrationCode = insurance.RegistrationCode;
                        }
                        else
                        {
                            DataBase.PatientInsuranceInstitutions.Delete(insuranceUpdate);
                            patientUpdate.PatientInsuranceInstitutions.Remove(insuranceUpdate);
                        }
                    }
                }

                DataBase.Contacts.Update(patientUpdate.Contact);
                DataBase.Patients.Update(patientUpdate);
                DataBase.Save();                
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
        public ActionResult MedicationPrint(int patientId)
        {
            Models.Pro.Professional pro = DataBase.Professionals.Get(p=>p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalOffices,ProfessionalOffices.ContactLocation.City,ProfessionalSpecializations.Specialization").SingleOrDefault();
                      
            return View(pro);
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeletePatient(int professionalPatientId)
        {
            try{

                ProfessionalPatient pPatient = DataBase.ProfessionalPatients.GetByID(professionalPatientId);
                pPatient.State = (byte)PatientState.Inactive;
                DataBase.ProfessionalPatients.Update(pPatient);
                DataBase.Save();

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
                Models.Medical.Patient patientInsert = new Models.Medical.Patient();
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

                DataBase.Contacts.Insert(patientInsert.Contact);
                DataBase.Patients.Insert(patientInsert);
                DataBase.ProfessionalPatients.Insert(professionalPatient);

                Models.Pro.Customer customer = new Models.Pro.Customer();
                customer.Contact = patientInsert.Contact;

                DataBase.Customers.Insert(customer);

                DataBase.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        PatientId = patientInsert.PatientId,
                        Success = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        PatientCount = DataBase.Professionals.GetPatientCount(professionalId)
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
            return DataBase.ProfessionalPatients.Get(p => p.PatientId == patientId && 
                p.ProfessionalId == Sic.Apollo.Session.ProfessionalId && p.State == active, 
                includeProperties: "Patient.Contact,Patient.PatientInsuranceInstitutions,Patient.PatientVitalSigns,Patient.PatientFiles").SingleOrDefault();
        }

        private void PrepareEpicrisis(Models.Medical.Patient patient)
        {
            DateTime currentDate = Sic.Web.Mvc.Session.CurrentDateTime;
            ViewBag.ProfessionalOption = ProfessionalOption.Epicrisis;
            ViewBag.PatientId = patient.PatientId;

            Models.Pro.Professional pro = DataBase.Professionals.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalOffices.City,ProfessionalSpecializations.Specialization").SingleOrDefault();

            /*For Medical Print*/
            ViewBag.Professional = pro;
            ViewBag.PatientName = patient.Contact.FullName;

            #region Insurance

            int insuranceAllow = 2;
            var insurances = DataBase.InsuranceInstitutions.Get(p => p.Active);

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

            var vitalSigns = DataBase.VitalSigns.Get(p => p.Active).OrderBy(p => p.Priority);
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

            var physicalExaminations = DataBase.PhysicalExaminations.Get(p => p.Active).OrderBy(p => p.Priority);
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

            var medicalProblems = DataBase.MedicalProblems.Get(p => p.Active).OrderBy(P=>P.Priority);
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
            var patiens = DataBase.Patients.GetPatientPerson(term, Sic.Apollo.Session.ProfessionalId);

            var result = patiens.Select(p => new { label = p.FullName, id = p.ContactId }).Take(15);

            // Return the result set as JSON
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
    }
}

