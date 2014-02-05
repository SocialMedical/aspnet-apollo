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
    public class SpecializationController : Sic.Web.Mvc.Controllers.BaseController
    {
        private ContextService db = new ContextService();

        //
        // GET: /Specialization/
        [Authorize(UserType.Administrator)]
        public ViewResult Index()
        {
            return View(db.Specializations.Get().ToList());
        }

        //
        // GET: /Specialization/Details/5
        [Authorize(UserType.Administrator)]
        public ViewResult Details(int id)
        {
            Specialization specialization = db.Specializations.GetByID(id);
            return View(specialization);
        }

        //
        // GET: /Specialization/Create
        [Authorize(UserType.Administrator)]
        public ActionResult Create()
        {
            var item = new Specialization();
            item.OnCreate();
            return View(item);
        } 

        //
        // POST: /Specialization/Create

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult Create(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                db.Specializations.Insert(specialization);
                db.Save();
                return RedirectToAction("Index");  
            }

            return View(specialization);
        }
        
        //
        // GET: /Specialization/Edit/5
        [Authorize(UserType.Administrator)]
        public ActionResult Edit(int id)
        {
            Specialization specialization = db.Specializations.GetByID(id);
            return View(specialization);
        }

        //
        // POST: /Specialization/Edit/5

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult Edit(Specialization specialization)
        {
            if (ModelState.IsValid)
            {                
                db.Specializations.Update(specialization);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(specialization);
        }

        //
        // GET: /Specialization/Delete/5
        [Authorize(UserType.Administrator)]
        public ActionResult Delete(int id)
        {
            Specialization specialization = db.Specializations.GetByID(id);
            return View(specialization);
        }

        //
        // POST: /Specialization/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(UserType.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {                        
            Specialization specialization = db.Specializations.GetByID(id);
            db.Specializations.Delete(specialization);
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