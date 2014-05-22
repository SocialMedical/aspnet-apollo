using Sic.Apollo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Controllers
{
    public class BaseController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        public bool IsValidProfesionalPatient(int patientId, bool addMessageValidation = false)
        {
            bool valid = DataBase.ProfessionalPatients.Exists(p => p.ProfessionalId == this.ProfessionalId && p.PatientId == patientId);
            if (!valid && addMessageValidation)
                this.AddErrorMessage(Sic.Apollo.Resources.MessageFor.ValidationEditProfessionalPatientNotFound);

            return valid;
        }

        public int ProfessionalId
        {
            get
            {
                return Sic.Apollo.Session.ProfessionalId;
            }
        }        
	}
}