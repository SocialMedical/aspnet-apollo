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
    public class MedicalHistoryController : BaseController
    {
        public JsonResult UpdateMedicalProblem(int patientId, int medicalProblemId, string description)
        {
            try
            {
                if (IsValidProfesionalPatient(patientId, true))
                {
                    MedicalHistory medicalHistory = DataBase.MedicalHistories.Get(p => 
                        p.PatientId == patientId &&
                        p.ProfessionalId == this.ProfessionalId &&
                        p.MedicalProblemId == medicalProblemId).SingleOrDefault();

                    if (medicalHistory == null)
                    {
                        medicalHistory = new MedicalHistory();
                        medicalHistory.MedicalProblemId = medicalProblemId;
                        medicalHistory.Description = description;
                        medicalHistory.ProfessionalId = this.ProfessionalId;
                        medicalHistory.RecordDate = GetCurrentDateTime();
                        medicalHistory.PatientId = patientId;

                        DataBase.MedicalHistories.Insert(medicalHistory);
                    }
                    else
                    {
                        medicalHistory.Description = description;
                        DataBase.MedicalHistories.Update(medicalHistory);
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


        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]        
        public JsonResult UpdateMedicalProblems(int patientId, byte medicalProblemType, List<MedicalHistory> medicalProblems)
        {
            try
            {               
                List<MedicalHistory> medicalProblemUpdate = DataBase.MedicalHistories.Get(p=> 
                    p.ProfessionalId == this.ProfessionalId &&
                    p.PatientId == patientId &&
                    p.MedicalProblem.Type == medicalProblemType).ToList();
                
                medicalProblems.RemoveAll(p => string.IsNullOrWhiteSpace(p.Description));
                DateTime currentDateTime = GetCurrentDateTime();

                foreach(var m in medicalProblems)
                {
                    m.PatientId = patientId;
                    m.ProfessionalId = this.ProfessionalId;                    
                    m.RecordDate = currentDateTime;
                }


                DataBase.MedicalHistories.Update(medicalProblems, medicalProblemUpdate, includeProperties:
                    new string[] { 
                                "Description"                        
                            }
                    );

                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    medicalProblemValues.Add(new MedicalHistory
                //    {
                //        MedicalHistoryId = Convert.ToInt32(row["pid"]),
                //        MedicalProblemId = Convert.ToInt16(row["mdpi"]),
                //        PatientId = patientId,
                //        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                //        Description = Convert.ToString(row["pd"])
                //    });
                //}
                

                //foreach (var medicalProblem in medicalProblemValues)
                //{
                //    if (medicalProblem.MedicalHistoryId == 0 && !string.IsNullOrWhiteSpace(medicalProblem.Description))
                //    {
                //        medicalProblem.RecordDate = currentDateTime;
                //        DataBase.MedicalHistories.Insert(medicalProblem);
                //    }
                //    else if (medicalProblem.MedicalHistoryId != 0)
                //    {
                //        if (!string.IsNullOrWhiteSpace(medicalProblem.Description))
                //        {
                //            DataBase.MedicalHistories.Update(medicalProblem);
                //            medicalProblem.RecordDate = currentDateTime;
                //        }
                //        else
                //            DataBase.MedicalHistories.Delete(medicalProblem);
                //    }
                //}

                DataBase.Save();

                this.AddDefaultSuccessMessage();                
            }
            catch (Exception)
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
	}
}