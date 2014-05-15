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
    public class PhysicalExaminationController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
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

                List<PatientPhysicalExamination> currentPhysicalExaminations = DataBase.PatientPhysicalExaminations.Get(p => p.PatientId == patientId &&
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
                        DataBase.PatientPhysicalExaminations.Insert(patientPhysicalExaminationUpdate);
                    }
                    else if (patientPhysicalExaminationUpdate.PatientPhysicalExaminationId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(patientPhysicalExaminationUpdate.Examination))
                        {
                            DataBase.PatientPhysicalExaminations.Update(patientPhysicalExaminationUpdate);
                            patientPhysicalExaminationUpdate.RecordDate = currentDateTime;
                        }
                        else
                            DataBase.PatientPhysicalExaminations.Delete(patientPhysicalExaminationUpdate);
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
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error }
                };
            }
        }
	}
}