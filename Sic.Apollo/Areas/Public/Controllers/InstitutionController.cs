using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Web.Mvc.Controllers;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Areas.Public.Controllers
{
    public class InstitutionController : BaseController
    {
        ContextService db = new ContextService();

        public ActionResult CommunityInstitutionsAutocomplete(string term)
        {
            var result = db.Institutions.Get(x =>
                    x.Contact.FirstName.ToLower().Contains(term.ToLower())).Select(p => new { label = p.Contact.FirstName, id = p.InstitutionId }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SchoolInstitutionsAutocomplete(string term)
        {
            var result = db.Institutions.Get(x =>
                    x.Contact.FirstName.ToLower().Contains(term.ToLower())).Select(p => new { label = p.Contact.FirstName, id = p.InstitutionId }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExperienceInstitutionsAutocomplete(string term)
        {
            var result = db.Institutions.Get(x =>
                    x.Contact.FirstName.ToLower().Contains(term.ToLower())).Select(p => new { label = p.Contact.FirstName, id = p.InstitutionId }).Take(15);

            // Return the result set as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
