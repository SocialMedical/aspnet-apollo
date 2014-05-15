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
    public class MedicalHistoryController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
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
                        DataBase.MedicalHistories.Insert(medicalProblem);
                    }
                    else if (medicalProblem.MedicalHistoryId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(medicalProblem.Description))
                        {
                            DataBase.MedicalHistories.Update(medicalProblem);
                            medicalProblem.RecordDate = currentDateTime;
                        }
                        else
                            DataBase.MedicalHistories.Delete(medicalProblem);
                    }
                }

                DataBase.Save();

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
	}
}