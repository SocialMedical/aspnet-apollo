using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Medical.Controllers
{
    public class VitalSignController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public ActionResult VitalSignHistory(int patientId)
        {
            return PartialView(DataBase.PatientVitalSigns.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                && p.PatientId == patientId, includeProperties: "VitalSign"));
        }


        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public JsonResult SaveVitalSign(int patientId, string vitalSigns, long dateTicks)
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

                List<PatientVitalSign> currentVitalSigns = DataBase.PatientVitalSigns.Get(p => p.PatientId == patientId &&
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
                        DataBase.PatientVitalSigns.Insert(patientVitalSignUpdate);
                    }
                    else if (patientVitalSignUpdate.PatientVitalSignId != 0)
                    {
                        if (!string.IsNullOrWhiteSpace(patientVitalSignUpdate.Value))
                        {
                            DataBase.PatientVitalSigns.Update(patientVitalSignUpdate);
                            patientVitalSignUpdate.VitalSignDate = date;
                            patientVitalSignUpdate.RecordDate = currentDateTime;
                        }
                        else
                            DataBase.PatientVitalSigns.Delete(patientVitalSignUpdate);
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
