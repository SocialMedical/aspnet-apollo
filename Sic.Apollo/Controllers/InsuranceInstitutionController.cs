using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models;

namespace Sic.Apollo.Controllers
{
    public class InsuranceInstitutionController : Sic.Web.Mvc.Controllers.BaseController
    {
        private ContextService db = new ContextService();

        //
        // GET: /InsuranceInstitution/
        [Authorize(UserType.Administrator)]
        public ViewResult Index()
        {            
            return View(db.InsuranceInstitutions.Get(p => p.Active, "Contact").ToList());  
        }

        //
        // GET: /InsuranceInstitution/Details/5
        [Authorize(UserType.Administrator)]
        public ViewResult Details(int id)
        {
            InsuranceInstitution insuranceinstitution = db.InsuranceInstitutions.GetByID(id);
            return View(insuranceinstitution);
        }

        //
        // GET: /InsuranceInstitution/Create
        [Authorize(UserType.Administrator)]
        public ActionResult Create()
        {
            var item = new InsuranceInstitution();
            item.OnCreate();
            return View(item);
        } 

        //
        // POST: /InsuranceInstitution/Create

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult Create(InsuranceInstitution insuranceinstitution)
        {            
            if (ModelState.IsValid)
            {
                db.InsuranceInstitutions.Insert(insuranceinstitution);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(insuranceinstitution);
        }
        
        //
        // GET: /InsuranceInstitution/Edit/5
        [Authorize(UserType.Administrator)]
        public ActionResult Edit(int id)
        {           
            InsuranceInstitution insuranceinstitution = db.InsuranceInstitutions.GetByID(id);
            return View(insuranceinstitution);
        }

        //
        // POST: /InsuranceInstitution/Edit/5

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult Edit(InsuranceInstitution insuranceinstitution)
        {            
            if (ModelState.IsValid)
            {
                db.InsuranceInstitutions.Update(insuranceinstitution);
                db.Contacts.Update(insuranceinstitution.Contact);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(insuranceinstitution);
        }

        //
        // GET: /InsuranceInstitution/Delete/5
        [Authorize(UserType.Administrator)]
        public ActionResult Delete(int id)
        {
            InsuranceInstitution insuranceinstitution = db.InsuranceInstitutions.GetByID(id);
            return View(insuranceinstitution);
        }

        //
        // POST: /InsuranceInstitution/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(UserType.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {                        
            InsuranceInstitution insuranceinstitution = db.InsuranceInstitutions.GetByID(id);
            db.Contacts.Delete(insuranceinstitution.Contact);
            db.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}