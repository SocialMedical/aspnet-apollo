using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Web.Mvc;
using Sic.Apollo.Models;

namespace Sic.Apollo.Controllers
{
    public class MedicalController : Sic.Web.Mvc.Controllers.BaseController
    {
        //
        // GET: /Medical/
        ContextService db = new ContextService();

        [ChildAction]
        [Authorize(UserType.Professional,UserType.Assistant)]
        public JsonResult VademecumsAutocomplete(string term)
        {
            var vademecums = db.Professionals.FindVademecums(term, Sic.Apollo.Session.ProfessionalId);

            var result = vademecums.Select(p => new { label = p.Name, gid = p.VademecumId, id = p.ProfessionalVademecumId, pos = (p.Posology??string.Empty) }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
