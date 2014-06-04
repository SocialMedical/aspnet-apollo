using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models;

namespace Sic.Apollo.Areas.Public.Controllers
{
    public class CustomerController : Sic.Web.Mvc.Controllers.BaseController
    {
        private ContextService db = new ContextService();

        public static string DefaultAction = "Profile";

        [Authorize(UserType.Customer)]
        public ActionResult Profile()
        {
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                int userId = Sic.Web.Mvc.Session.UserId;
                Models.Pro.Customer customer = db.Customers.Get(p => p.CustomerId == userId,
                    includeProperties: "Contact").Single();

                customer.Contact.AssingDateParts();

                return View(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
       
        [HttpPost]
        public ActionResult Profile(Models.Pro.Customer customer)
        {
            Models.Pro.Customer customerUpdate = db.Customers.Get(p => p.CustomerId == customer.CustomerId,
                        includeProperties: "Contact").Single();

            TryUpdateModel(customerUpdate);

            customerUpdate.Validate(ModelState);

            if (ModelState.IsValid)
            {                                
                db.Customers.Update(customerUpdate);
                db.Save();
            }
            
            return View(customerUpdate);
        }

        public ActionResult ProfilePicture()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}