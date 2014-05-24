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
    public class VitalSignController : BaseController
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ValidateInput(false)]
        public ActionResult VitalSignsHistory(int patientId)
        {
            return PartialView("_VitalSignsHistory", DataBase.PatientVitalSigns.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                && p.PatientId == patientId, includeProperties: "VitalSign"));
        }


        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [PreventSpam(2)]
        public JsonResult UpdateVitalSigns(int patientId, List<PatientVitalSign> vitalSigns, long dateTicks)
        {
            try
            {
                if (IsValidProfesionalPatient(patientId, true))
                {
                    DateTime dateEdit = new DateTime(dateTicks);                                  
                   
                    foreach (PatientVitalSign v in vitalSigns)
                    {
                        v.PatientId = patientId;
                        v.ProfessionalId = this.ProfessionalId;                  
                    }

                    DateTime currentDateTime = this.GetCurrentDateTime();                    

                    List<PatientVitalSign> currentVitalSigns = DataBase.PatientVitalSigns.Get(p => p.PatientId == patientId &&
                        p.VitalSignDate == dateEdit.Date).ToList();

                    foreach (var patientVitalSign in vitalSigns)
                    {
                        PatientVitalSign patientVitalSignUpdate = currentVitalSigns.FirstOrDefault(p => p.VitalSignId == patientVitalSign.VitalSignId);

                        if (patientVitalSignUpdate == null)
                            patientVitalSignUpdate = patientVitalSign;
                        else
                            patientVitalSignUpdate.Value = patientVitalSign.Value;

                        if (patientVitalSignUpdate.PatientVitalSignId == 0 && !string.IsNullOrWhiteSpace(patientVitalSignUpdate.Value))
                        {
                            patientVitalSignUpdate.VitalSignDate = dateEdit;
                            patientVitalSignUpdate.RecordDate = currentDateTime;
                            if (!DataBase.PatientVitalSigns.Exists(p => p.PatientId == patientId && p.VitalSignDate == dateEdit.Date && 
                                p.VitalSignId == patientVitalSignUpdate.VitalSignId))
                                DataBase.PatientVitalSigns.Insert(patientVitalSignUpdate);
                            else
                                DataBase.PatientVitalSigns.Update(patientVitalSignUpdate);
                        }
                        else if (patientVitalSignUpdate.PatientVitalSignId != 0)
                        {
                            if (!string.IsNullOrWhiteSpace(patientVitalSignUpdate.Value))
                            {
                                DataBase.PatientVitalSigns.Update(patientVitalSignUpdate);
                                patientVitalSignUpdate.VitalSignDate = dateEdit;
                                patientVitalSignUpdate.RecordDate = currentDateTime;
                            }
                            else
                                DataBase.PatientVitalSigns.Delete(patientVitalSignUpdate);
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
