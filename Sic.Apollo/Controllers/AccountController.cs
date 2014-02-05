using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models.Security;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Pro;
using System.Web.Security;
using Sic.Web.Mvc;
using Sic.Apollo.Models.Appointment;

namespace Sic.Apollo.Controllers
{
    public class AccountController : Sic.Web.Mvc.Controllers.BaseController
    {
        private ContextService db = new ContextService();                        
                        
        public ActionResult AccessDenied()
        {                       
            return View();
        }

        public ActionResult ExpiredSession()
        {
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }


        public ActionResult ChangePassword()
        {
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                User user = db.Users.GetByID(Sic.Web.Mvc.Session.UserId);
                Models.Security.View.ChangePassword changePassword = new Models.Security.View.ChangePassword();
                changePassword.LogonName = user.LogonName;
                changePassword.UserId = user.UserId;
                return View(changePassword);
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }

        [HttpPost]                
        public ActionResult ChangePassword(Models.Security.View.ChangePassword changePassword)
        {            
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.GetByID(Sic.Web.Mvc.Session.UserId);
                    if (user.Password != changePassword.Password)
                        this.ModelState.AddModelError("Password", Sic.Apollo.Resources.Resources.LabelForIncorrectPassword);
                    else
                    {
                        user.Password = changePassword.NewPassword;
                        user.ConfirmedPassword = changePassword.NewPassword;
                        db.Users.Update(user);
                        db.Save();

                        return LoginVerify(new SignIn() { LoginFor = SignIn.LoginAction.Login, LogonName = user.LogonName, Password = user.Password });
                    }                    
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
            return View(changePassword);
        }

        #region SignIn       
        
        public ActionResult LoginButton()
        {
            return PartialView();
        }        

        public ActionResult Logout()
        {            
            AssingUserSessionValues(null);            
            return RedirectToAction("Index", "Home");
        }              

        [ChildAction]
        public ActionResult Login()
        {
            return PartialView(new SignIn()
                {
                    LoginFor = Sic.Apollo.Models.Security.SignIn.LoginAction.Login
                });
        }

        [ChildAction]
        public ActionResult LoginPopUp()
        {
            return PartialView(new SignIn()
            {
                LoginFor = Sic.Apollo.Models.Security.SignIn.LoginAction.PopUp
            });
        }

        [HttpPost]
        public ActionResult LoginVerify(SignIn signIn , int? appointmentId = null, int? specializationId = null)
        {            
            var user = db.Users.Verify(signIn.LogonName, signIn.Password);

            bool isValid = false;

            string message = string.Empty;
            string location = string.Empty;            
            if (user != null)
            {
                isValid = true;
                AssingUserSessionValues(user);

                if (Sic.Web.Mvc.Session.UserType == (int)Sic.UserType.Professional
                    || Sic.Web.Mvc.Session.UserType == (int)Sic.UserType.Assistant)
                {
                    location = Url.Action(AppointmentController.DefaultProfessionalAction, "Appointment");                    
                }
                else if (Sic.Web.Mvc.Session.UserType == (int)Sic.UserType.Customer)
                {
                    //location = Url.Action(AppointmentController.DefaultCustomerAction, "Appointment");
                    location = Url.Action("Index","Home");                    
                }
            }
            else
            {
                message = Sic.Apollo.Resources.Resources.LabelForLoginFailed;
                ModelState.AddModelError("LogonName", message);
            }

            if (signIn.LoginFor == SignIn.LoginAction.PopUp)
            {                
                return Json(new { Success = isValid, Message = message, Location = location });                
            }
            else if (isValid && signIn.LoginFor == SignIn.LoginAction.Login)
            {
                return Redirect(location);
            }
            else if (isValid && signIn.LoginFor == SignIn.LoginAction.Appointment)
            {
                return RedirectToAction("Start", "Appointment", new { appointmentId = appointmentId, specializationId = specializationId });
            }
            else if (isValid)
            {
                string url = Sic.Web.Mvc.Session.UrlSecureLastAttempted;
                if (string.IsNullOrEmpty(url))
                    url = "/";
                return Redirect(url);
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }

        private void AssingUserSessionValues(User user)
        {
            if (user != null)
            {
                Sic.Web.Mvc.Session.IsLogged = true;
                Sic.Web.Mvc.Session.UserId = user.UserId;
                Sic.Web.Mvc.Session.LogonName = user.LogonName;
                Sic.Web.Mvc.Session.FullName = user.Contact.FullName;
                Sic.Web.Mvc.Session.UserType = user.Type;
                Sic.Web.Mvc.Session.UserState = user.State;
                if (Sic.Web.Mvc.Session.UserType == (int)UserType.Professional)
                    Sic.Apollo.Session.ProfessionalId = user.UserId;
                else if (Sic.Web.Mvc.Session.UserType == (int)UserType.Assistant)
                {
                    var professionalTeam = db.ProfessionalTeams.Get(p => p.Active && p.TeamUserId == user.UserId).FirstOrDefault();
                    if(professionalTeam!=null)
                        Sic.Apollo.Session.ProfessionalId = professionalTeam.ProfessionalId;
                }
                else
                    Sic.Apollo.Session.ProfessionalId = 0;
            }
            else
            {
                Sic.Web.Mvc.Session.IsLogged = false;
                Sic.Web.Mvc.Session.UserId = 0;
                Sic.Web.Mvc.Session.LogonName = null;
                Sic.Web.Mvc.Session.FullName = null;
                Sic.Web.Mvc.Session.UserType = 0;
                Sic.Web.Mvc.Session.UserState = 0;
                Sic.Apollo.Session.ProfessionalId = 0;
            }
        }

        #endregion SignIn

        public ActionResult SelectUserType()
        {
            return View();
        }
        
        public ActionResult RegisterProfessional()
        {
            return View(new Sic.Apollo.Models.Security.User() { ConfirmedPassword="" });
        }                

        public JsonResult ValidateLogonName(string logonName)
        {            
            return Json(!db.Users.UserExists(logonName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateEditLogonName(string logonName)
        {
            bool result = !db.Users.EditUserExists(logonName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult RegisterProfessional(User user)
        {
            if (string.IsNullOrEmpty(user.Contact.PhoneNumber))
            {
                ModelState.AddModelError("Contact.PhoneNumber", Sic.Apollo.Resources.Resources.ValidationFieldRequired);
            }

            user.Validate(ModelState);

            user.Contact.Validate(ModelState, true);
            user.OnCreate();
            user.Contact.Email = user.LogonName;
            user.Type = (int)UserType.Professional;
            user.State = (int)UserState.PendingConfirmation;
            user.RegisterDate = Sic.Web.Mvc.Session.CurrentDateTime;

            Professional professional = new Professional();
            professional.OnCreate();
            professional.Contact = user.Contact;

            if (ModelState.IsValid)
            {                
                db.Users.Insert(user);
                db.Professionals.Insert(professional);
                db.Save();

                AssingUserSessionValues(user);

                Mail.SendWelcome(UserType.Professional, user.Contact.FullName2, user.Contact.Email, (Gender)user.Contact.Gender);

                return RedirectToAction("Profile", "Professional");
            }

            return View(user);
        }
        
        public ActionResult RegisterCustomer(string email = null, int? appointmentId = null, int? specializationId = null)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.SpecializationId = specializationId;

            return View(new User() { LogonName = email, ConfirmedPassword = "",
                Contact = new Models.General.Contact() { Email = email } 
            });
        }
        
        public ActionResult AppointmentRegisterCustomer(SignIn signIn, int appointmentId, int specializationId)
        {
            return RedirectToAction("RegisterCustomer", new
            {
                email = signIn.LogonName,
                appointmentId = appointmentId,
                specializationId = specializationId
            });
        }
        
        [HttpPost]
        public ActionResult RegisterCustomer(User user, int? appointmentId = null, int? specializationId = null)
        {
            user.Contact.Validate(ModelState, true);
            user.Validate(ModelState);
            if (ModelState.IsValid)
            {                
                user.Contact.Email = user.LogonName;                
                user.Type = (int)UserType.Customer;
                user.State = (int)UserState.Active;
                user.RegisterDate = Sic.Web.Mvc.Session.CurrentDateTime;

                Customer customer = new Customer();
                customer.OnCreate();
                customer.Contact = user.Contact;
                
                db.Users.Insert(user);
                db.Customers.Insert(customer);
                db.Save();

                AssingUserSessionValues(user);

                Mail.SendWelcome(UserType.Customer, user.Contact.FullName2, user.Contact.Email, (Gender)user.Contact.Gender);

                if (appointmentId == null)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Start", "Appointment", new { appointmentId = appointmentId, specializationId = specializationId });               
            }

            return View(user);
        }

       
    }
}
