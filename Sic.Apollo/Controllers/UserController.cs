using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models.Security;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Pro;

namespace Sic.Apollo.Controllers
{
    public class UserController : Controller
    {
        private ContextService db = new ContextService();
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }


        // GET: /User/CreateProfessional
        public ActionResult CreateProfessional()
        {
            var item = new User();
            item.OnCreate();
            return View(item);
        }

        //
        // POST: /User/CreateProfessional
        [HttpPost]
        public ActionResult CreateProfessional(User user)
        {
            if (ModelState.IsValid)
            {
                user.LogonName = user.Contact.Email;
                user.Type = (int)UserType.Professional;

                Professional professional = new Professional();
                professional.OnCreate();
                professional.Contact = user.Contact;

                db.Users.Insert(user);
                db.Professionals.Insert(professional);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: /User/CreateCustomer
        public ActionResult CreateCustomer()
        {
            var item = new User();
            item.OnCreate();
            return View(item);
        }

        //
        // POST: /User/CreateCustomer
        [HttpPost]
        public ActionResult CreateCustomer(User user)
        {
            if (ModelState.IsValid)
            {
                user.LogonName = user.Contact.Email;
                user.Type = (int)UserType.Customer;

                Customer customer = new Customer();
                customer.OnCreate();
                customer.Contact = user.Contact;
                
                db.Users.Insert(user);
                db.Customers.Insert(customer);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
