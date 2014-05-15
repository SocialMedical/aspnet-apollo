using Sic.Apollo.Areas.Public.Controllers;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Appointment.View;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models.Security;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Public.Controllers
{
    public class BookingController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        public static string DefaultProfessionalAction = "ProfessionalBook";
        public static string DefaultCustomerAction = "CustomerBook";

        #region Search

        public ActionResult Search(string locationsId, int specializationId,
            long? startTime = null, bool simple = false, int? reschudeleAppointmentTransactionId = null)
        {
            ViewBag.DaysModel = Utils.DayOfWeek(startTime);

            DateTime? startDate = Sic.Web.Mvc.Session.CurrentDateTime;
            if (startTime != null)
                startDate = new DateTime(startTime.Value);

            ViewBag.StartDate = startDate.Value;
            ViewBag.VisibleDays = 7;

            ViewBag.QueryParams = "?locationsId=" + locationsId;
            ViewBag.LocationsId = locationsId;
            ViewBag.SpecializationValue = specializationId;
            ViewBag.Simple = simple;
            if (reschudeleAppointmentTransactionId != null)
            {
                ViewBag.Reschudele = true;
                ViewBag.ReschudeleAppointmentTransactionId = reschudeleAppointmentTransactionId;
            }

            if (locationsId.Length > 0)
            {
                int[] locationsSearch = locationsId.Split('|').Select(p => Convert.ToInt32(p)).ToArray();
                return PartialView("_Horary", DataBase.Appointments.GetAppointments(locationsSearch, startDate, ViewBag.VisibleDays));
            }
            else
            {
                return PartialView("_Horary", new List<ContactLocationAppointments>());
            }
        }

        #endregion

        #region BookAppointment

        [ChildAction]
        public ActionResult PersonalAppointment(int contactLocationId, int specializationId, long? reschudeleAppointmentTransactionId = null)
        {
            ViewBag.StartDate = Sic.Web.Mvc.Session.CurrentDateTime;
            ViewBag.VisibleDays = 7;
            ViewBag.LocationsId = contactLocationId;

            var query = DataBase.Professionals.GetProfessionals(specialityId: specializationId, contactLocationId: contactLocationId).SingleOrDefault();

            if (reschudeleAppointmentTransactionId == null)
            {
                ViewBag.Reschudele = false;
                return PartialView(query);
            }
            else
            {
                ViewBag.Reschudele = true;
                ViewBag.ReschudeleAppointmentTransactionId = reschudeleAppointmentTransactionId;
                return PartialView(query);
            }
        }

        

        private bool AvailableAppointment(Models.Appointment.Appointment appointment)
        {
            return appointment != null && (appointment.State == (byte)AppointmentState.Pending)
                && appointment.StartDate > Sic.Web.Mvc.Session.CurrentDateTime.AddMinutes(5);
        }


        public ActionResult Start(long appointmentId, int specializationId)
        {
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                var appointment = DataBase.Appointments.Get(p => p.AppointmentId == appointmentId).SingleOrDefault();

                var appointmentTransaction = new Models.Appointment.View.AppointmentTransaction();

                appointmentTransaction.AppointmentId = appointmentId;
                appointmentTransaction.CustomerId = Sic.Web.Mvc.Session.UserId;
                appointmentTransaction.ProfessionalId = appointment != null ? appointment.ProfessionalId : 0;
                appointmentTransaction.ContactLocationId = appointment != null ? appointment.ContactLocationId : 0;
                appointmentTransaction.SpecializationId = specializationId;
                appointmentTransaction.Professional = DataBase.Professionals.GetProfessionals(specialityId: specializationId, professionalId: appointment.ProfessionalId, contactLocationId: appointment.ContactLocationId).Single();
                appointmentTransaction.Customer = DataBase.Customers.GetByID(Sic.Web.Mvc.Session.UserId);
                appointmentTransaction.AppointmentDate = appointment.StartDate;
                appointmentTransaction.Step = 1;

                if (!AvailableAppointment(appointment))
                    return View("NotAvailable", appointmentTransaction);

                if (DataBase.AppointmentTransactions.VerifyMaxAppointmentsOpen(appointment.StartDate))
                    return View("CustomerMaximumAllowedExceeded");

                var reasonList = DataBase.SpecializationAppointmentReasons.Get(p => p.SpecializationId == specializationId).OrderBy(p => p.DescriptionName).ToSelectList();
                if (!reasonList.Any())
                {
                    return RedirectToAction("ResourceNotFound", "Error");
                }

                ViewBag.SpecializationAppointmentReasonList = reasonList;
                ViewBag.InsuranceInstitutionList = DataBase.Professionals.GetInsuranceInstitutions(appointment.ProfessionalId).OrderBy(p => p.DescriptionName).ToSelectList();

                return View(appointmentTransaction);
            }
            else
                return RedirectToAction("SignIn", new { appointmentId = appointmentId, specializationId = specializationId });
        }

        [Authorize(UserType.Customer)]
        [HttpPost]
        public ActionResult Start(Models.Appointment.View.AppointmentTransaction appointmentTransaction)
        {
            return RedirectDetails(appointmentTransaction);
        }

        [Authorize(UserType.Customer)]
        public ActionResult Reschedule(int appointmentId, int? reschudeleAppointmentTransactionId)
        {
            if (Sic.Web.Mvc.Session.IsLogged)
                return RedirectToAction("CustomerRescheduleConfirmation", new { appointmentId = appointmentId, reschudeleAppointmentTransactionId = reschudeleAppointmentTransactionId });
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(long appointmentId, int professionalId, int customerId, int specializationId, int specializationAppointmentReasonId,
            byte applyInsurance, int? applyInsuranceInstitutionId = 0)
        {
            Models.Appointment.Appointment appointment = DataBase.Appointments.GetByID(appointmentId);

            Models.Appointment.View.AppointmentTransaction appointmentTransaction = new Models.Appointment.View.AppointmentTransaction();
            appointmentTransaction.AppointmentId = appointmentId;
            appointmentTransaction.ProfessionalId = professionalId;
            appointmentTransaction.AppointmentDate = appointment.StartDate;
            appointmentTransaction.SpecializationAppointmentReasonId = specializationAppointmentReasonId;
            appointmentTransaction.CustomerId = customerId;
            appointmentTransaction.SendMeReminder = true;
            appointmentTransaction.ContactLocationId = appointment != null ? appointment.ContactLocationId : 0;
            appointmentTransaction.SpecializationId = specializationId;
            appointmentTransaction.Professional = DataBase.Professionals.GetProfessionals(specialityId: specializationId, professionalId: appointment.ProfessionalId, contactLocationId: appointment.ContactLocationId).Single();
            appointmentTransaction.Customer = DataBase.Customers.GetByID(Sic.Web.Mvc.Session.UserId);
            appointmentTransaction.Step = 2;

            if (!AvailableAppointment(appointment))
                return View("NotAvailable", appointmentTransaction);

            if (DataBase.AppointmentTransactions.VerifyMaxAppointmentsOpen(appointment.StartDate))
                return View("CustomerMaximumAllowedExceeded");

            appointmentTransaction.InsuranceInstitutionId = applyInsuranceInstitutionId == 0 ? null : applyInsuranceInstitutionId;
            if (appointmentTransaction.InsuranceInstitutionId != null)
                appointmentTransaction.InsuranceInstitution = DataBase.InsuranceInstitutions.GetByID(appointmentTransaction.InsuranceInstitutionId);

            appointmentTransaction.UseInsurance = Convert.ToBoolean(applyInsurance);
            appointmentTransaction.ContactPhoneNumber = appointmentTransaction.Customer.Contact.PhoneNumber;
            appointmentTransaction.SpecializationAppointmentReason = DataBase.SpecializationAppointmentReasons.GetByID(appointmentTransaction.SpecializationAppointmentReasonId);

            return View(appointmentTransaction);
        }

        private ActionResult RedirectDetails(Models.Appointment.View.AppointmentTransaction appointmentTransaction)
        {
            return RedirectToAction("Details", new
            {
                appointmentId = appointmentTransaction.AppointmentId,
                professionalId = appointmentTransaction.ProfessionalId,
                customerId = appointmentTransaction.CustomerId,
                specializationId = appointmentTransaction.SpecializationId,
                specializationAppointmentReasonId = appointmentTransaction.SpecializationAppointmentReasonId,
                applyInsurance = appointmentTransaction.UseInsurance ? 1 : 0,
                applyInsuranceInstitutionId = appointmentTransaction.InsuranceInstitutionId ?? 0
            });
        }

        [Authorize(UserType.Customer)]
        public ActionResult Finished(int appointmentTransactionId)
        {
            Models.Appointment.AppointmentTransaction appointmentTransactionConfirm = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId, includeProperties: "Appointment,InsuranceInstitution.Contact,SpecializationAppointmentReason,Customer,Customer.Contact").SingleOrDefault();

            if (appointmentTransactionConfirm == null || appointmentTransactionConfirm.CustomerId != Sic.Web.Mvc.Session.UserId)
            {
                return RedirectToAction("Profile", "Customer");
            }

            Models.Appointment.View.AppointmentTransaction appointmentTransaction = new Models.Appointment.View.AppointmentTransaction()
            {
                AppointmentId = appointmentTransactionConfirm.AppointmentId,
                ProfessionalId = appointmentTransactionConfirm.Appointment.ProfessionalId,
                SpecializationAppointmentReasonId = appointmentTransactionConfirm.SpecializationAppointmentReasonId,
                AppointmentDate = appointmentTransactionConfirm.Appointment.StartDate,
                CustomerId = appointmentTransactionConfirm.CustomerId,
                SendMeReminder = appointmentTransactionConfirm.SendMeReminder,
                ContactLocationId = appointmentTransactionConfirm.Appointment.ContactLocationId,
                SpecializationId = appointmentTransactionConfirm.SpecializationAppointmentReason.SpecializationId,
                Professional = DataBase.Professionals.GetProfessionals(specialityId: appointmentTransactionConfirm.SpecializationAppointmentReason.SpecializationId,
                                professionalId: appointmentTransactionConfirm.Appointment.ProfessionalId, contactLocationId: appointmentTransactionConfirm.Appointment.ContactLocationId).Single(),
                ContactPhoneNumber = appointmentTransactionConfirm.ContactPhoneNumber,
                InsuranceInstitutionId = appointmentTransactionConfirm.InsuranceInstitutionId,
                InsuranceInstitution = appointmentTransactionConfirm.InsuranceInstitution,
                UseInsurance = appointmentTransactionConfirm.UseInsurance,
                Customer = appointmentTransactionConfirm.Customer,
                SpecializationAppointmentReason = appointmentTransactionConfirm.SpecializationAppointmentReason,
                Step = 3
            };

            return View(appointmentTransaction);
        }

        [HttpPost]
        [Authorize(UserType.Customer)]
        public ActionResult Details(Models.Appointment.View.AppointmentTransaction appointmentTransaction)
        {
            if (ModelState.IsValid)
            {
                var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransaction.AppointmentId).Single(); //, includeProperties: "Professional,Professional.Contact").Single();

                if (!AvailableAppointment(appointmentUpdate))
                    return View("NotAvailable", appointmentTransaction);

                appointmentUpdate.State = (int)AppointmentState.PendingConfirmation;

                var appointmentTransactionInsert = new Models.Appointment.AppointmentTransaction()
                {
                    CustomerId = appointmentTransaction.CustomerId,
                    SpecializationAppointmentReasonId = appointmentTransaction.SpecializationAppointmentReasonId,
                    InsuranceInstitutionId = appointmentTransaction.InsuranceInstitutionId,
                    ContactPhoneNumber = appointmentTransaction.ContactPhoneNumber,
                    SendMeReminder = appointmentTransaction.SendMeReminder,
                    CustomerNotes = appointmentTransaction.CustomerNotes,
                    State = (int)AppointmentState.PendingConfirmation,
                    TransactionDate = Sic.Web.Mvc.Session.CurrentDateTime,
                    AppointmentForMe = true,
                    UseInsurance = appointmentTransaction.UseInsurance,
                    Appointment = appointmentUpdate
                };

                if (Sic.Web.Mvc.Session.UserType != (int)UserType.Customer)
                {
                    var customer = DataBase.Customers.Get(p => p.CustomerId == Sic.Web.Mvc.Session.UserId).FirstOrDefault();

                    if (customer == null)
                    {
                        DataBase.Customers.Insert(new Customer() { CustomerId = Sic.Web.Mvc.Session.UserId });
                    }
                }

                if (DataBase.CustomerProfessionals.Get(p => p.CustomerId == Sic.Web.Mvc.Session.UserId && p.ProfessionalId == appointmentUpdate.ProfessionalId).Count() == 0)
                {
                    DataBase.CustomerProfessionals.Insert(new CustomerProfessional() { CustomerId = Sic.Web.Mvc.Session.UserId, ProfessionalId = appointmentUpdate.ProfessionalId });
                }

                DataBase.Appointments.Update(appointmentUpdate);
                DataBase.AppointmentTransactions.Insert(appointmentTransactionInsert);

                DataBase.Save();

                var professionalContact = DataBase.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();
                var customerContact = DataBase.Contacts.Get(p => p.ContactId == appointmentTransactionInsert.CustomerId).FirstOrDefault();
                var professionalOffice = DataBase.ProfessionalOffices.Get(p => p.ContactLocationId == appointmentUpdate.ContactLocationId).FirstOrDefault();

                Mail.SendAppointmentBooked(UserType.Professional, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionInsert.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);

                Mail.SendAppointmentBooked(UserType.Customer, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionInsert.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);

                return RedirectToAction("Finished", new { appointmentTransactionId = appointmentTransactionInsert.AppointmentTransactionId });
            }

            return View(appointmentTransaction);
        }

        public ActionResult SignIn(long appointmentId, int specializationId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.SpecializationId = specializationId;

            return View(new SignIn()
            {
                LoginFor = Sic.Apollo.Models.Security.SignIn.LoginAction.Appointment
            });
        }

        [ChildAction]
        public ActionResult NewCustomer(long appointmentId, int specializationId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.SpecializationId = specializationId;

            return PartialView(new SignIn());
        }

        [ChildAction]
        public ActionResult Login(long appointmentId, int specializationId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.SpecializationId = specializationId;
            ViewBag.LoginPopUp = false;

            return PartialView(new SignIn());
        }

        #endregion BookAppointment
	}
}