using Sic.Apollo.Controllers;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class PhysicalExaminationController : BaseController
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]        
        public ActionResult History(int patientId)
        {
            if (this.IsValidProfesionalPatient(patientId))
            {
                return PartialView("_History", DataBase.PatientPhysicalExaminations.Get(p =>
                    p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                    && p.PatientId == patientId, "PhysicalExamination"));
            }
            return Json();
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult Update(long dateTicks, PatientPhysicalExamination physicalExamination)
        {
            try
            {
                if (IsValidProfesionalPatient(physicalExamination.PatientId, true))
                {
                    DateTime dateSet = new DateTime(dateTicks).Date;

                    PatientPhysicalExamination physicalExaminationUpdate = DataBase.PatientPhysicalExaminations.Get(p => 
                            p.ProfessionalId == this.ProfessionalId
                        && p.PatientId == physicalExamination.PatientId 
                        && p.PhysicalExaminationId == physicalExamination.PhysicalExaminationId
                        && p.ExaminationDate == dateSet).SingleOrDefault();

                    if (physicalExaminationUpdate == null)
                    {
                        physicalExaminationUpdate = new PatientPhysicalExamination();
                        physicalExaminationUpdate.ProfessionalId = this.ProfessionalId;
                        physicalExaminationUpdate.PatientId = physicalExamination.PatientId;                        
                        physicalExaminationUpdate.ExaminationDate = dateSet;
                        physicalExaminationUpdate.PhysicalExaminationId = physicalExamination.PhysicalExaminationId;
                        DataBase.PatientPhysicalExaminations.Insert(physicalExaminationUpdate);
                    }
                    else
                    {
                        DataBase.PatientPhysicalExaminations.Update(physicalExaminationUpdate);
                    }

                    physicalExaminationUpdate.RecordDate = this.GetCurrentDateTime();
                    physicalExaminationUpdate.Examination = physicalExamination.Examination;

                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                }
                
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return Json();
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]        
        public JsonResult UpdateAll(int patientId, long dateTicks, List<PatientPhysicalExamination> physicalExaminations)
        {
            try
            {
                if (IsValidProfesionalPatient(patientId, true))
                {
                    List<PatientPhysicalExamination> patientPhysicalExaminations = new List<PatientPhysicalExamination>();
                    foreach (PatientPhysicalExamination row in physicalExaminations)
                    {
                        patientPhysicalExaminations.Add(new PatientPhysicalExamination
                        {
                            PatientPhysicalExaminationId = row.PatientPhysicalExaminationId,
                            PatientId = patientId,
                            PhysicalExaminationId = row.PhysicalExaminationId,
                            ProfessionalId = this.ProfessionalId,
                            Examination = row.Examination
                        });
                    }

                    DateTime currentDateTime = this.GetCurrentDateTime();
                    DateTime dateSet = new DateTime(dateTicks).Date;

                    List<PatientPhysicalExamination> currentPhysicalExaminations = DataBase.PatientPhysicalExaminations.Get(p => 
                        p.ProfessionalId == this.ProfessionalId &&
                        p.PatientId == patientId &&
                        p.ExaminationDate == dateSet).ToList();

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
                            patientPhysicalExaminationUpdate.ExaminationDate = dateSet;
                            patientPhysicalExaminationUpdate.RecordDate = currentDateTime;
                            DataBase.PatientPhysicalExaminations.Insert(patientPhysicalExaminationUpdate);
                        }
                        else if (patientPhysicalExaminationUpdate.PatientPhysicalExaminationId != 0)
                        {
                            if (!string.IsNullOrWhiteSpace(patientPhysicalExaminationUpdate.Examination))
                            {
                                DataBase.PatientPhysicalExaminations.Update(patientPhysicalExaminationUpdate);
                                patientPhysicalExaminationUpdate.ExaminationDate = dateSet;
                                patientPhysicalExaminationUpdate.RecordDate = currentDateTime;
                            }
                            else
                                DataBase.PatientPhysicalExaminations.Delete(patientPhysicalExaminationUpdate);
                        }
                    }

                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
	}
}