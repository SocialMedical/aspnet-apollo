using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Appointment;
using Sic.Apollo.Models.Appointment.View;
using System.Threading;
using System.Text;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models.Security;
using Sic.Apollo.Models.Repositories;
using Sic.Web.Mvc;

namespace Sic.Apollo.Controllers
{
    public class AppointmentController : Sic.Web.Mvc.Controllers.BaseController
    {
        ContextService db = new ContextService();

        public static string DefaultProfessionalAction = "ProfessionalBook";
        public static string DefaultCustomerAction = "CustomerBook";

        #region Search

        public ActionResult Search(string locationsId, int specializationId,
            long? startTime = null, bool simple = false, int? reschudeleAppointmentTransactionId = null)
        {
            ViewBag.DaysModel = DayOfWeek(startTime);

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
                return PartialView("Horary", db.Appointments.GetAppointments(locationsSearch, startDate, ViewBag.VisibleDays));
            }
            else
            {
                return PartialView("Horary", new List<ContactLocationAppointments>());
            }
        }

        #endregion

        public IEnumerable<Apollo.Models.Appointment.View.DayOfWeek> DayOfWeek(long? startTime = null)
        {
            DateTime? date = null;
            if (startTime != null)
                date = new DateTime(startTime.Value);

            return GetDaysOfWeek(date);
        }

        private IEnumerable<Apollo.Models.Appointment.View.DayOfWeek> GetDaysOfWeek(DateTime? startDate = null, bool fixedCurrentDate = true)
        {
            DateTime date = Sic.Web.Mvc.Session.CurrentDateTime;
            if (startDate != null && (fixedCurrentDate || startDate.Value > date))
                date = startDate.Value;

            List<Apollo.Models.Appointment.View.DayOfWeek> wa = new List<Apollo.Models.Appointment.View.DayOfWeek>();
            for (int i = 1; i <= 7; i++)
            {
                DateTime dateStart = date.Date;
                if (dateStart.Date == Sic.Web.Mvc.Session.CurrentDateTime.Date)
                    dateStart = Sic.Web.Mvc.Session.CurrentDateTime;

                wa.Add(new Apollo.Models.Appointment.View.DayOfWeek(dateStart));
                date = date.AddDays(1);
            }

            return wa.OrderBy(p => p.Date);
        }

        #region BookAppointment

        [HttpPost]
        [Authorize(UserType.Professional,UserType.Assistant)]
        public JsonResult Create(int contactLocationId, long startDate,
            long endDate, string customerNameReference, bool saveAsNewPatient, string notes, int patientId = 0)
        {
            try
            {                
                DateTime startDateTime = new DateTime(startDate);
                DateTime endDateTime = new DateTime(endDate);
                int stateAvailable = (int)AppointmentState.Pending;
                Appointment appointment = db.Appointments.Get(p => p.StartDate == startDateTime && p.State == (int)stateAvailable
                    && p.ProfessionalId == Sic.Apollo.Session.ProfessionalId).FirstOrDefault();

                if (appointment == null)
                {
                    appointment = new Appointment()
                    {
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        ContactLocationId = contactLocationId,
                        StartDate = startDateTime,
                        EndDate = endDateTime                        
                    };
                    db.Appointments.Insert(appointment);
                }
                else
                {
                    db.Appointments.Update(appointment);
                }

                appointment.State = (int)AppointmentState.Confirmed;
                Models.Appointment.AppointmentTransaction transaction = new Models.Appointment.AppointmentTransaction();
                transaction.Appointment = appointment;
                transaction.TransactionDate = Sic.Web.Mvc.Session.CurrentDateTime;

                
                if (patientId != 0)
                {
                    Customer customer = db.Customers.GetByID(patientId);
                    if (customer == null)
                    {
                        customer = new Customer();
                        customer.CustomerId = patientId;
                        db.Customers.Insert(customer);
                    }
                    transaction.CustomerId = customer.CustomerId;
                }
                else if (saveAsNewPatient)
                {
                    string[] names = customerNameReference.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 
                    Models.Medical.Patient patient = new Models.Medical.Patient()
                    {
                        Contact = new Models.General.Contact
                        {
                            FirstName = names[0],
                            LastName = names.Count()>1?names[1]: string.Empty
                        },                        
                    };

                    db.Contacts.Insert(patient.Contact);
                    db.Patients.Insert(patient);

                    Models.Medical.ProfessionalPatient professionalPatient = new Models.Medical.ProfessionalPatient();
                    professionalPatient.Patient = patient;
                    professionalPatient.State = 1;
                    professionalPatient.ProfessionalId = appointment.ProfessionalId;

                    db.ProfessionalPatients.Insert(professionalPatient);

                    Customer customer = new Customer();
                    customer.Contact = patient.Contact;
                    transaction.Customer = customer;

                    db.Customers.Insert(customer);                    
                }

                transaction.CustomerNameReference = customerNameReference;
                transaction.CustomerNotes = notes;
                transaction.State = (int)AppointmentState.Confirmed;

                db.AppointmentTransactions.Insert(transaction);

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForNewAppointmentCreated, transaction.CustomerNameReference, appointment.StartDate.ToDefaultDateTimeFormat()),
                        Success = true,
                        AppointmentTransactionId = transaction.AppointmentTransactionId,
                        PatientCount = db.Professionals.GetPatientCount(appointment.ProfessionalId),
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(appointment.ProfessionalId)
                    }
                };
            }
            catch(Exception)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure,
                        Success = false
                    }
                };
            }
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult Delete(long appointmentTransactionId)
        {
            try
            {
                Models.Appointment.AppointmentTransaction app = db.AppointmentTransactions.Get(p=>p.AppointmentTransactionId == appointmentTransactionId,
                    includeProperties:"Appointment").SingleOrDefault();

                app.Appointment.State = (int)AppointmentState.Pending;

                db.Appointments.Update(app.Appointment);
                db.AppointmentTransactions.Delete(app);                                                               
                
                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForAppointmentDeleted),
                        Success = true,                        
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(Sic.Apollo.Session.ProfessionalId)
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure,
                        Success = false
                    }
                };
            }
        }

        private bool AvailableAppointment(Appointment appointment)
        {
            return appointment != null && (appointment.State == (byte)AppointmentState.Pending)
                && appointment.StartDate > Sic.Web.Mvc.Session.CurrentDateTime.AddMinutes(5);
        }


        public ActionResult Start(long appointmentId, int specializationId)
        {
            if (Sic.Web.Mvc.Session.IsLogged)
            {
                var appointment = db.Appointments.Get(p => p.AppointmentId == appointmentId).SingleOrDefault();

                var appointmentTransaction = new Models.Appointment.View.AppointmentTransaction();

                appointmentTransaction.AppointmentId = appointmentId;
                appointmentTransaction.CustomerId = Sic.Web.Mvc.Session.UserId;
                appointmentTransaction.ProfessionalId = appointment != null ? appointment.ProfessionalId : 0;
                appointmentTransaction.ContactLocationId = appointment != null ? appointment.ContactLocationId : 0;
                appointmentTransaction.SpecializationId = specializationId;
                appointmentTransaction.Professional = db.Professionals.GetProfessionals(specialityId: specializationId, professionalId: appointment.ProfessionalId, contactLocationId: appointment.ContactLocationId).Single();
                appointmentTransaction.Customer = db.Customers.GetByID(Sic.Web.Mvc.Session.UserId);
                appointmentTransaction.AppointmentDate = appointment.StartDate;
                appointmentTransaction.Step = 1;

                if (!AvailableAppointment(appointment))
                    return View("NotAvailable", appointmentTransaction);

                if (db.AppointmentTransactions.VerifyMaxAppointmentsOpen(appointment.StartDate))
                    return View("CustomerMaximumAllowedExceeded");

                var reasonList = db.SpecializationAppointmentReasons.Get(p => p.SpecializationId == specializationId).OrderBy(p => p.DescriptionName).ToSelectList();
                if (!reasonList.Any())
                {
                    return RedirectToAction("ResourceNotFound", "Error");
                }

                ViewBag.SpecializationAppointmentReasonList = reasonList;
                ViewBag.InsuranceInstitutionList = db.Professionals.GetInsuranceInstitutions(appointment.ProfessionalId).OrderBy(p => p.DescriptionName).ToSelectList();

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
            Appointment appointment = db.Appointments.GetByID(appointmentId);

            Models.Appointment.View.AppointmentTransaction appointmentTransaction = new Models.Appointment.View.AppointmentTransaction();
            appointmentTransaction.AppointmentId = appointmentId;
            appointmentTransaction.ProfessionalId = professionalId;
            appointmentTransaction.AppointmentDate = appointment.StartDate;
            appointmentTransaction.SpecializationAppointmentReasonId = specializationAppointmentReasonId;
            appointmentTransaction.CustomerId = customerId;
            appointmentTransaction.SendMeReminder = true;
            appointmentTransaction.ContactLocationId = appointment != null ? appointment.ContactLocationId : 0;
            appointmentTransaction.SpecializationId = specializationId;
            appointmentTransaction.Professional = db.Professionals.GetProfessionals(specialityId: specializationId, professionalId: appointment.ProfessionalId, contactLocationId: appointment.ContactLocationId).Single();
            appointmentTransaction.Customer = db.Customers.GetByID(Sic.Web.Mvc.Session.UserId);
            appointmentTransaction.Step = 2;

            if (!AvailableAppointment(appointment))
                return View("NotAvailable", appointmentTransaction);

            if (db.AppointmentTransactions.VerifyMaxAppointmentsOpen(appointment.StartDate))
                return View("CustomerMaximumAllowedExceeded");

            appointmentTransaction.InsuranceInstitutionId = applyInsuranceInstitutionId == 0 ? null : applyInsuranceInstitutionId;
            if (appointmentTransaction.InsuranceInstitutionId != null)
                appointmentTransaction.InsuranceInstitution = db.InsuranceInstitutions.GetByID(appointmentTransaction.InsuranceInstitutionId);

            appointmentTransaction.UseInsurance = Convert.ToBoolean(applyInsurance);
            appointmentTransaction.ContactPhoneNumber = appointmentTransaction.Customer.Contact.PhoneNumber;
            appointmentTransaction.SpecializationAppointmentReason = db.SpecializationAppointmentReasons.GetByID(appointmentTransaction.SpecializationAppointmentReasonId);

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
            Models.Appointment.AppointmentTransaction appointmentTransactionConfirm = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId, includeProperties: "Appointment,InsuranceInstitution.Contact,SpecializationAppointmentReason,Customer,Customer.Contact").SingleOrDefault();

            if (appointmentTransactionConfirm == null || appointmentTransactionConfirm.CustomerId != Sic.Web.Mvc.Session.UserId)
            {
                return RedirectToAction(CustomerController.DefaultAction, "Customer");
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
                Professional = db.Professionals.GetProfessionals(specialityId: appointmentTransactionConfirm.SpecializationAppointmentReason.SpecializationId,
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
                var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentTransaction.AppointmentId).Single(); //, includeProperties: "Professional,Professional.Contact").Single();

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
                    var customer = db.Customers.Get(p => p.CustomerId == Sic.Web.Mvc.Session.UserId).FirstOrDefault();

                    if (customer == null)
                    {
                        db.Customers.Insert(new Customer() { CustomerId = Sic.Web.Mvc.Session.UserId });
                    }
                }

                if (db.CustomerProfessionals.Get(p => p.CustomerId == Sic.Web.Mvc.Session.UserId && p.ProfessionalId == appointmentUpdate.ProfessionalId).Count() == 0)
                {
                    db.CustomerProfessionals.Insert(new CustomerProfessional() { CustomerId = Sic.Web.Mvc.Session.UserId, ProfessionalId = appointmentUpdate.ProfessionalId });
                }

                db.Appointments.Update(appointmentUpdate);
                db.AppointmentTransactions.Insert(appointmentTransactionInsert);

                db.Save();

                var professionalContact = db.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();
                var customerContact = db.Contacts.Get(p => p.ContactId == appointmentTransactionInsert.CustomerId).FirstOrDefault();
                var professionalOffice = db.ProfessionalOffices.Get(p => p.ContactLocationId == appointmentUpdate.ContactLocationId).FirstOrDefault();

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

        #region CustomerAppointment

        [Authorize(UserType.Customer)]
        public ActionResult CustomerMyAppointments()
        {
            return View(db.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId));
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerPendingRatings()
        {
            return View(db.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId, new List<int>() { (int)AppointmentState.Attended }));
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerRate(Sic.Apollo.Models.Appointment.View.CustomerAppointment customerAppointment)
        {
            customerAppointment.RateScore1 = 3;
            customerAppointment.RateScore2 = 3;
            customerAppointment.RateScore3 = 3;

            return View(customerAppointment);
        }

        [HttpPost]
        public ActionResult CustomerRate(long appointmentTransactionId, int? rateScore1, int? rateScore2, int? rateScore3, string rateComments)
        {
            var appointmentTransactionUpdate = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
            appointmentTransactionUpdate.State = (int)AppointmentState.Rated;

            appointmentTransactionUpdate.RateScore1 = rateScore1;
            appointmentTransactionUpdate.RateScore2 = rateScore2;
            appointmentTransactionUpdate.RateScore3 = rateScore3;
            appointmentTransactionUpdate.RateComments = rateComments;
            appointmentTransactionUpdate.RateDate = Sic.Web.Mvc.Session.CurrentDateTime;

            var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Rated;

            var score = db.Professionals.GetScore(appointmentUpdate.ProfessionalId);

            if (score != null)
            {
                score.CountRateScore++;

                if (score.CountRateScore >= 3)
                {
                    var professional = db.Professionals.Get(p => p.ProfessionalId == appointmentUpdate.ProfessionalId).Single();

                    professional.RateScore1 = (score.SumRateScore1 + rateScore1) / score.CountRateScore;
                    professional.RateScore2 = (score.SumRateScore2 + rateScore2) / score.CountRateScore;
                    professional.RateScore3 = (score.SumRateScore3 + rateScore3) / score.CountRateScore;

                    professional.RateScore = (professional.RateScore1 + professional.RateScore2 + professional.RateScore3) / 3;

                    db.Professionals.Update(professional);
                }
            }

            db.Appointments.Update(appointmentUpdate);
            db.AppointmentTransactions.Update(appointmentTransactionUpdate);

            db.Save();

            var contact = db.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();
            int rate = (rateScore1.Value + rateScore2.Value + rateScore3.Value) / 3;

            return RedirectToAction("CustomerRated", new { professionalId = appointmentUpdate.ProfessionalId, fullName = contact.FullName, rate = rate });
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerRated(int professionalId, string fullName, int rate)
        {
            ViewBag.ProfessionalId = professionalId;
            ViewBag.FullName = fullName;
            ViewBag.Rate = rate;

            return View();
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerCancel(Sic.Apollo.Models.Appointment.View.CustomerAppointment customerAppointment)
        {
            return View(customerAppointment);
        }

        [HttpPost]
        [Authorize(UserType.Customer)]
        public ActionResult CustomerCancel(long appointmentTransactionId)
        {
            var appointmentTransactionUpdate = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
            appointmentTransactionUpdate.State = (int)AppointmentState.Canceled;

            var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Pending;

            db.Appointments.Update(appointmentUpdate);
            db.AppointmentTransactions.Update(appointmentTransactionUpdate);

            db.Save();

            return RedirectToAction("CustomerCanceled", new { professionalId = appointmentUpdate.ProfessionalId });
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerCanceled(int professionalId)
        {
            ViewBag.ProfessionalId = professionalId;

            return View();
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerReschedule(Sic.Apollo.Models.Appointment.View.CustomerAppointment customerAppointment)
        {
            return View(customerAppointment);
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerRescheduleConfirmation(long appointmentId, long reschudeleAppointmentTransactionId)
        {
            var appointment = db.Appointments.Get(p => p.AppointmentId == appointmentId).Single();

            ViewBag.ReschudeleAppointmentTransactionId = reschudeleAppointmentTransactionId;

            return View(appointment);
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerRescheduleBook(long appointmentId, long reschudeleAppointmentTransactionId)
        {
            var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.PendingConfirmation;

            var appointmentTransaction = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == reschudeleAppointmentTransactionId).Single();
            appointmentTransaction.State = (int)AppointmentState.Canceled;
            appointmentTransaction.CancelTransactionDate = Sic.Web.Mvc.Session.CurrentDateTime;

            var appointmentUpdateCancel = db.Appointments.Get(p => p.AppointmentId == appointmentTransaction.AppointmentId).Single();
            appointmentUpdateCancel.State = (int)AppointmentState.Pending;

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

            db.Appointments.Update(appointmentUpdate);
            db.Appointments.Update(appointmentUpdateCancel);
            db.AppointmentTransactions.Update(appointmentTransaction);
            db.AppointmentTransactions.Insert(appointmentTransactionInsert);

            db.Save();

            var contact = db.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();

            return RedirectToAction("CustomerRescheduled",
                new
                {
                    phoneNumber = contact.PhoneNumber,
                    professionalId = appointmentUpdate.ProfessionalId
                });
        }

        [Authorize(UserType.Customer)]
        private ActionResult CustomerRescheduled(string phoneNumber, int professionalId)
        {
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.ProfessionalId = professionalId;

            return View();
        }

        #endregion CustomerAppointment

        #region ProfessionalAppointment

        [ChildAction]
        public ActionResult PersonalAppointment(int contactLocationId, int specializationId, long? reschudeleAppointmentTransactionId = null)
        {
            ViewBag.StartDate = Sic.Web.Mvc.Session.CurrentDateTime;
            ViewBag.VisibleDays = 7;
            ViewBag.LocationsId = contactLocationId;

            var query = db.Professionals.GetProfessionals(specialityId: specializationId, contactLocationId: contactLocationId).SingleOrDefault();

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

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalBook()
        {
            var schedules = db.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);

            ViewBag.Schedules = schedules;

            if (schedules.Count() > 0)
            {
                ViewBag.Duration = schedules.Min(p => p.AppointmentDuration);
                ViewBag.StartTime = schedules.Min(p => p.StartTime);
                ViewBag.EndTime = schedules.Max(p => p.EndTime).AddMinutes(-ViewBag.Duration);
            }
            else
            {
                ViewBag.Duration = 30;
                ViewBag.StartTime = new DateTime(1900, 1, 1, 9, 0, 0);
                ViewBag.EndTime = new DateTime(1900, 1, 1, 17, 30, 0);
            }

            ViewBag.ProfessionalOption = ProfessionalOption.ProfessionalBook;
            return View(db.ProfessionalOffices.Get(p => p.ContactId == Sic.Apollo.Session.ProfessionalId));
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerBook()
        {
            HomeController.FillSearchCriteria(this, db);

            var viewModel = new Models.Pro.View.Customer()
            {
                CustomerId = Sic.Web.Mvc.Session.UserId
            };

            viewModel.Appointments = db.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId);
            viewModel.CustomerProfessionals = db.Customers.GetCustomerProfessionals(Sic.Web.Mvc.Session.UserId);

            return View(viewModel);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult ProfessionalBookDetails(int contactLocationId, long startDate, int duration, long startTime, long endTime)
        {
            ViewBag.DaysModel = DayOfWeek(startDate);

            DateTime startDateTime = new DateTime(startDate);
            DateTime endDateTime = startDateTime.AddDays(7);

            ViewBag.Duration = duration;
            ViewBag.StartTime = new DateTime(startTime);
            ViewBag.EndTime = new DateTime(endTime);

            ViewBag.ContactLocationId = contactLocationId;
            ViewBag.StartDate = startDateTime.Date;
            ViewBag.EndDate = endDateTime;

            var schedules = db.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);
            ViewBag.Schedules = schedules;

            return PartialView(db.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, contactLocationId == 0 ? (int?)null : contactLocationId,
                startDateTime, endDateTime,
                null));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult AppointmentBookItem(int appointmentTransactionId, long startDate, bool isForNew = false)
        {
            ViewBag.CurrentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;
            string viewName = "AppointmentBookItem";
            ViewBag.StartDate = new DateTime(startDate).Date;
            if (isForNew)
                viewName = "AppointmentBookItemForNew";
            return PartialView(viewName,db.AppointmentTransactions.GetProfessionalAppointments(appointmentTransactionId: appointmentTransactionId).SingleOrDefault());
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmAttendancePending()
        {
            return View(db.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.Confirmed }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalAppointmentsHistorial()
        {
            return View(db.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.Attended, (int)AppointmentState.NotAttended, (int)AppointmentState.Rated }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult PendingConfirmToAttention()
        {
            return View(db.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.PendingConfirmation }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult PendingConfirmToAttentionList(int? contactLocationId = null)
        {
            return View(db.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId,
                new List<int>() { (int)AppointmentState.PendingConfirmation }, contactLocationId));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmToAttention(Sic.Apollo.Models.Appointment.View.ProfessionalAppointment professionalAppointment)
        {
            return View(professionalAppointment);
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult ConfirmAllToAttention(string appointmentTransactionId, string confirmToAttentionNotes)
        {
            try
            {
                var apps = appointmentTransactionId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (apps.Any())
                {
                    var appsUpdate = apps.Select(p => Convert.ToInt64(p));
                    var appointmentTransactionsUpdate = db.AppointmentTransactions.Get(p => appsUpdate.Contains(p.AppointmentTransactionId),
                        includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice");

                    foreach (var appointmentTransactionUpdate in appointmentTransactionsUpdate)
                        ConfirmAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                }
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelConfirmToAttention,
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForAllConfirmAppointmentToAttention)
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult CancelAllToAttention(string appointmentTransactionId, string confirmToAttentionNotes)
        {
            try
            {
                var apps = appointmentTransactionId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (apps.Any())
                {
                    var appsUpdate = apps.Select(p => Convert.ToInt64(p));
                    var appointmentTransactionsUpdate = db.AppointmentTransactions.Get(p => appsUpdate.Contains(p.AppointmentTransactionId),
                        includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice");

                    foreach (var appointmentTransactionUpdate in appointmentTransactionsUpdate)
                        CancelAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);
                }
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelCancelToAttention,
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForAllCancelAppointmentToAttention)
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure }
                };
            }
        }

        private void ConfirmAppointmentToAttention(Models.Appointment.AppointmentTransaction appointmentTransactionUpdate, string confirmToAttentionNotes)
        {
            appointmentTransactionUpdate.State = (int)AppointmentState.Confirmed;
            appointmentTransactionUpdate.ConfirmToAttentionNotes = confirmToAttentionNotes;

            var appointmentUpdate = appointmentTransactionUpdate.Appointment;//db.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Confirmed;

            db.Appointments.Update(appointmentUpdate);
            db.AppointmentTransactions.Update(appointmentTransactionUpdate);            

            db.Save();

            if (appointmentTransactionUpdate.Customer != null)
            {
                var professionalContact = appointmentTransactionUpdate.Appointment.Professional.Contact;
                var customerContact = appointmentTransactionUpdate.Customer.Contact;
                var professionalOffice = appointmentTransactionUpdate.Appointment.ProfessionalOffice;

                Mail.SendAppointmentConfirmed(UserType.Professional, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionUpdate.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);

                Mail.SendAppointmentConfirmed(UserType.Customer, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionUpdate.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);
            }
        }

        private void CancelAppointmentToAttention(Models.Appointment.AppointmentTransaction appointmentTransactionUpdate, string confirmToAttentionNotes)
        {
            appointmentTransactionUpdate.State = (int)AppointmentState.Canceled;
            appointmentTransactionUpdate.ConfirmToAttentionNotes = confirmToAttentionNotes;            
            appointmentTransactionUpdate.CancelTransactionDate = Sic.Web.Mvc.Session.CurrentDateTime;

            var appointmentUpdate = appointmentTransactionUpdate.Appointment;
            appointmentUpdate.State = (int)AppointmentState.Canceled;

            db.Appointments.Update(appointmentUpdate);
            db.AppointmentTransactions.Update(appointmentTransactionUpdate);

            db.Save();

            if (appointmentTransactionUpdate.Customer != null && !string.IsNullOrWhiteSpace(appointmentTransactionUpdate.Customer.Contact.Email))
            {
                var professionalContact = appointmentTransactionUpdate.Appointment.Professional.Contact;
                var customerContact = appointmentTransactionUpdate.Customer.Contact;
                var professionalOffice = appointmentTransactionUpdate.Appointment.ProfessionalOffice;

                Mail.SendAppointmentCanceled(UserType.Professional, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionUpdate.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);

                Mail.SendAppointmentCanceled(UserType.Customer, professionalContact.FullName2, professionalContact.Email, (Gender)professionalContact.Gender,
                    professionalOffice.Address, professionalOffice.DefaultPhoneNumber,
                    customerContact.FullName2, customerContact.Email, (Gender)customerContact.Gender, appointmentTransactionUpdate.ContactPhoneNumber,
                    appointmentUpdate.StartDate, appointmentUpdate.EndDate);
            }
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult ConfirmToAttention(long appointmentTransactionId, string confirmToAttentionNotes)
        {
            try
            {
                var appointmentTransactionUpdate = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice").Single();

                ConfirmAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmToAttention,
                                        appointmentTransactionUpdate.Customer.Contact.FullName, appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                        ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelConfirmToAttention
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure }
                };
            }
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult CancelToAttention(long appointmentTransactionId, string confirmToAttentionNotes)
        {
            try
            {
                var appointmentTransactionUpdate =
                db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                    includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice").Single();

                CancelAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForCancelToAttention,
                            appointmentTransactionUpdate.Customer.Contact.FullName, appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                        ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelCancelToAttention
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalCanceled()
        {
            return View();
        }      

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult ConfirmAttended(long appointmentTransactionId, string attentionNotes)
        {
            try
            {
                var appointmentTransactionUpdate = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
                appointmentTransactionUpdate.State = (int)AppointmentState.Attended;
                appointmentTransactionUpdate.AttentionNotes = attentionNotes;

                var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
                appointmentUpdate.State = (int)AppointmentState.Attended;

                db.Appointments.Update(appointmentUpdate);
                db.AppointmentTransactions.Update(appointmentTransactionUpdate);                

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        //PatientCount = db.Professionals.GetPatientCount(appointment.ProfessionalId),
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId),
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmAttended,
                            appointmentTransactionUpdate.Customer !=null ? 
                            appointmentTransactionUpdate.Customer.Contact.FullName : appointmentTransactionUpdate.CustomerNameReference, 
                            appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat())
                        //,ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelCancelToAttention
                    }
                };
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }

        }

        //[Authorize(UserType.Professional)]
        //public ActionResult ProfessionalAttended()
        //{
        //    return View();
        //}

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmUnAttended(Sic.Apollo.Models.Appointment.View.ProfessionalAppointment professionalAppointment)
        {
            return View(professionalAppointment);
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmUnAttended(long appointmentTransactionId, string attentionNotes)
        {
            try
            {
                var appointmentTransactionUpdate = db.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
                appointmentTransactionUpdate.State = (int)AppointmentState.NotAttended;
                appointmentTransactionUpdate.AttentionNotes = attentionNotes;

                var appointmentUpdate = db.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
                appointmentUpdate.State = (int)AppointmentState.NotAttended;

                db.Appointments.Update(appointmentUpdate);
                db.AppointmentTransactions.Update(appointmentTransactionUpdate);

                db.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = db.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId),
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmUnAttended,
                            appointmentTransactionUpdate.Customer != null ?
                            appointmentTransactionUpdate.Customer.Contact.FullName : appointmentTransactionUpdate.CustomerNameReference, 
                            appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat())
                        //,ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelCancelToAttention
                    }
                };                
            }
            catch
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure
                    }
                };
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalNotAttended()
        {
            return View();
        }

        #endregion ProfessionalAppointment

        #region Schedule

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult SchedulePreview(int contactLocationId, string weekDays = "", long? startTimeOfDay = null, long? endTimeOfDay = null,
            int? appointmentDuration = 30, long? startDate = null, long? endDate = null,
            int eachWeek = 1, long? startConfiguration = null, int? professionalOfficeScheduleId = null, int visibleDays = 7)
        {
            ViewBag.DaysModel = DayOfWeek(startDate);

            var list = new List<Models.Appointment.View.ContactLocationAppointments>();

            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            var currentdatetime = Sic.Web.Mvc.Session.CurrentDateTime;

            if (startDate == null || new DateTime(startDate.Value) < currentdatetime)
                start = Sic.Web.Mvc.Session.CurrentDateTime;
            else
                start = new DateTime(startDate.Value).Date;

            if (professionalOfficeScheduleId == null)
            {
                startTime = new DateTime(startTimeOfDay.Value);
                endTime = new DateTime(endTimeOfDay.Value);

                if (endDate == null || (new DateTime(endDate.Value) - start).TotalDays > 7)
                    end = start.AddDays(visibleDays);
                else
                    end = new DateTime(endDate.Value);

                DateTime? startConfigurationDate = null;
                if (startConfiguration.HasValue)
                    startConfigurationDate = new DateTime(startConfiguration.Value);
                else
                    startConfigurationDate = start;

                var result = db.Appointments.GetPreviewAppointments(contactLocationId,
                    weekDays,
                    startTime,
                    endTime,
                    appointmentDuration.Value,
                    start, end, eachWeek, startConfigurationDate.Value, null);

                list.Add(result);
            }

            ViewBag.StartDate = start;
            ViewBag.InMaintenance = true;
            ViewBag.LocationsId = contactLocationId;
            ViewBag.WeekDays = weekDays;
            ViewBag.StartTimeOfDay = startTimeOfDay;
            ViewBag.EndTimeOfDay = endTimeOfDay;
            ViewBag.AppointmenDuration = appointmentDuration;
            ViewBag.EndDate = endDate;
            ViewBag.ForEachWeek = eachWeek;
            ViewBag.professionalOfficeScheduleId = professionalOfficeScheduleId;
            ViewBag.StartConfiguration = startConfiguration;

            return PartialView("Horary", list);
        }

        #endregion
    }
}
