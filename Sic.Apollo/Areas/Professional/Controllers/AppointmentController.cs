using Sic.Apollo.Areas.Public.Controllers;
using Sic.Apollo.Models;
using Sic.Apollo.Models.Appointment.View;
using Sic.Apollo.Models.Pro;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class AppointmentController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {                

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Book()
        {
            //var schedules = DataBase.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);
            //ViewBag.Schedules = schedules;

            //if (schedules.Count() > 0)
            //{
            //    ViewBag.Duration = schedules.Min(p => p.AppointmentDuration);
            //    ViewBag.StartTime = schedules.Min(p => p.StartTime);
            //    ViewBag.EndTime = schedules.Max(p => p.EndTime).AddMinutes(-ViewBag.Duration);
            //}
            //else
            //{
            //    ViewBag.Duration = 30;
            //    ViewBag.StartTime = new DateTime(1900, 1, 1, 9, 0, 0);
            //    ViewBag.EndTime = new DateTime(1900, 1, 1, 17, 30, 0);
            //}

            ViewBag.ProfessionalOption = ProfessionalOption.ProfessionalBook;
            return View(DataBase.ProfessionalOffices.Get(p => p.ContactId == Sic.Apollo.Session.ProfessionalId));
        }

        

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult BookDetails(int contactLocationId, long? startDate = null)
        {
            AppointmentParameters parameters = new AppointmentParameters();
            parameters.DaysOfWeek = Utils.DayOfWeek(startDate);

            IEnumerable<ProfessionalOfficeSchedule> schedules = null;
            if(contactLocationId == 0)
                schedules = DataBase.ProfessionalOfficeSchedules.Get(p => 
                    p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);
            else
                schedules = DataBase.ProfessionalOfficeSchedules.Get(p =>
                    p.ContactLocationId == contactLocationId &&
                    p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);

            if (schedules.Any())
            {
                parameters.Duration = schedules.Min(p => p.AppointmentDuration);
                parameters.StartTime = schedules.Min(p => p.StartTime);
                parameters.EndTime = schedules.Max(p => p.EndTime).AddMinutes(-parameters.Duration);
            }
            else
            {
                parameters.Duration = 30;
                parameters.StartTime = new DateTime(1900, 1, 1, 9, 0, 0);
                parameters.EndTime = new DateTime(1900, 1, 1, 17, 30, 0);
            }

            if (!startDate.HasValue)
                parameters.StartDate = GetCurrentDateTime();
            else
                parameters.StartDate = new DateTime(startDate.Value);
                                                     
            parameters.EndDate = parameters.StartDate.AddDays(7);
                        
            parameters.ContactLocationId = contactLocationId;
            parameters.StartDate = parameters.StartDate.Date;            
            
            parameters.Schedules = schedules;

            ViewBag.AppointmentParameters = parameters;

            return PartialView("_BookDetails", 
                DataBase.AppointmentTransactions.GetProfessionalAppointments(
                Sic.Apollo.Session.ProfessionalId, contactLocationId == 0 ? (int?)null : contactLocationId,
                parameters.StartDate, parameters.EndDate,
                null));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult BookItem(int appointmentTransactionId, long startDate, bool isForNew = false)
        {
            ViewBag.CurrentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;
            string viewName = "_BookItem";
            ViewBag.StartDate = new DateTime(startDate).Date;
            if (isForNew)
                viewName = "_BookItemForNew";
            return PartialView(viewName, DataBase.AppointmentTransactions.GetProfessionalAppointments(appointmentTransactionId: appointmentTransactionId).SingleOrDefault());
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult Create(int contactLocationId, long startDate,
            long endDate, string customerNameReference, bool saveAsNewPatient, string notes, int patientId = 0)
        {
            try
            {
                DateTime startDateTime = new DateTime(startDate);
                DateTime endDateTime = new DateTime(endDate);
                int stateAvailable = (int)AppointmentState.Pending;
                Models.Appointment.Appointment appointment = DataBase.Appointments.Get(p => p.StartDate == startDateTime && p.State == (int)stateAvailable
                    && p.ProfessionalId == Sic.Apollo.Session.ProfessionalId).FirstOrDefault();

                if (appointment == null)
                {
                    appointment = new Models.Appointment.Appointment()
                    {
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        ContactLocationId = contactLocationId,
                        StartDate = startDateTime,
                        EndDate = endDateTime
                    };
                    DataBase.Appointments.Insert(appointment);
                }
                else
                {
                    DataBase.Appointments.Update(appointment);
                }

                appointment.State = (int)AppointmentState.Confirmed;
                Models.Appointment.AppointmentTransaction transaction = new Models.Appointment.AppointmentTransaction();
                transaction.Appointment = appointment;
                transaction.TransactionDate = Sic.Web.Mvc.Session.CurrentDateTime;


                if (patientId != 0)
                {
                    Customer customer = DataBase.Customers.GetByID(patientId);
                    if (customer == null)
                    {
                        customer = new Customer();
                        customer.CustomerId = patientId;
                        DataBase.Customers.Insert(customer);
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
                            LastName = names.Count() > 1 ? names[1] : string.Empty
                        },
                    };

                    DataBase.Contacts.Insert(patient.Contact);
                    DataBase.Patients.Insert(patient);

                    Models.Medical.ProfessionalPatient professionalPatient = new Models.Medical.ProfessionalPatient();
                    professionalPatient.Patient = patient;
                    professionalPatient.State = 1;
                    professionalPatient.ProfessionalId = appointment.ProfessionalId;

                    DataBase.ProfessionalPatients.Insert(professionalPatient);

                    Customer customer = new Customer();
                    customer.Contact = patient.Contact;
                    transaction.Customer = customer;

                    DataBase.Customers.Insert(customer);
                }

                transaction.CustomerNameReference = customerNameReference;
                transaction.CustomerNotes = notes;
                transaction.State = (int)AppointmentState.Confirmed;

                DataBase.AppointmentTransactions.Insert(transaction);

                DataBase.Save();

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForNewAppointmentCreated, transaction.CustomerNameReference, appointment.StartDate.ToDefaultDateTimeFormat()),
                    Data.MessageType.Success,
                    Resources.MessageFor.TitleAppointmentCreated);

                return Json(new { Success = true,
                        AppointmentTransactionId = transaction.AppointmentTransactionId,
                        PatientCount = DataBase.Professionals.GetPatientCount(appointment.ProfessionalId),
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointment.ProfessionalId) }
                        );                                
            }
            catch (Exception)
            {
                this.AddDefaultErrorMessage();
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult Delete(long appointmentTransactionId)
        {
            try
            {
                Models.Appointment.AppointmentTransaction app = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                    includeProperties: "Appointment").SingleOrDefault();

                app.Appointment.State = (int)AppointmentState.Pending;

                DataBase.Appointments.Update(app.Appointment);
                DataBase.AppointmentTransactions.Delete(app);

                DataBase.Save();

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForAppointmentDeleted),
                    Data.MessageType.Success, Resources.MessageFor.TitleAppointmentDeleted);

                return Json(new {
                        Success = true,
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(Sic.Apollo.Session.ProfessionalId)
                    });                
            }
            catch
            {
                this.AddDefaultErrorMessage();
                
                return Json(new { Success = false });
            }
        }
        private void ConfirmAppointmentToAttention(Models.Appointment.AppointmentTransaction appointmentTransactionUpdate, string confirmToAttentionNotes)
        {
            appointmentTransactionUpdate.State = (int)AppointmentState.Confirmed;
            appointmentTransactionUpdate.ConfirmToAttentionNotes = confirmToAttentionNotes;

            var appointmentUpdate = appointmentTransactionUpdate.Appointment;//DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Confirmed;

            DataBase.Appointments.Update(appointmentUpdate);
            DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

            DataBase.Save();

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

            DataBase.Appointments.Update(appointmentUpdate);
            DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

            DataBase.Save();

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
                var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice").Single();

                ConfirmAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmToAttention,
                                        appointmentTransactionUpdate.Customer.Contact.FullName, appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                                        Data.MessageType.Success, Resources.MessageFor.TitleAppointmentForAttentionConfirmed);

                return Json(new{
                    Success = true,
                    AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
                    ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelConfirmToAttention
                });                
            }
            catch
            {
                this.AddDefaultErrorMessage();

                return Json(new { Success = false});                
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
                DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                    includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice").Single();

                CancelAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForCancelToAttention,
                            appointmentTransactionUpdate.Customer.Contact.FullName, appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                            Data.MessageType.Success, Resources.MessageFor.TitleAppointmentCanceled);

                return Json(new
                {
                    Success = true,
                    AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
                    ShortMessage = Sic.Apollo.Resources.Resources.MessageForShortLabelCancelToAttention
                });                
            }
            catch
            {
                this.AddDefaultErrorMessage();
                return Json(new { Success = false });                
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
                var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
                appointmentTransactionUpdate.State = (int)AppointmentState.Attended;
                appointmentTransactionUpdate.AttentionNotes = attentionNotes;

                var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
                appointmentUpdate.State = (int)AppointmentState.Attended;

                DataBase.Appointments.Update(appointmentUpdate);
                DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

                DataBase.Save();

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmAttended,
                            appointmentTransactionUpdate.Customer != null ?
                            appointmentTransactionUpdate.Customer.Contact.FullName : appointmentTransactionUpdate.CustomerNameReference,
                            appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                            Data.MessageType.Success, Resources.MessageFor.TitleAppointmentAttended);

                return Json(new {
                        Success = true,                        
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId)
                    });                
            }
            catch
            {
                this.AddDefaultErrorMessage();
                return Json(new { Success=false });                
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
                var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
                appointmentTransactionUpdate.State = (int)AppointmentState.NotAttended;
                appointmentTransactionUpdate.AttentionNotes = attentionNotes;

                var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
                appointmentUpdate.State = (int)AppointmentState.NotAttended;

                DataBase.Appointments.Update(appointmentUpdate);
                DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

                DataBase.Save();

                this.AddMessage(string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmUnAttended,
                            appointmentTransactionUpdate.Customer != null ?
                            appointmentTransactionUpdate.Customer.Contact.FullName : appointmentTransactionUpdate.CustomerNameReference,
                            appointmentTransactionUpdate.TransactionDate.ToDefaultDateTimeFormat()),
                            Data.MessageType.Success, Resources.MessageFor.TitleAppointmentNotAttended);

                return Json(new {
                        Success = true,
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId)                        
                    });                
            }
            catch
            {
                this.AddDefaultErrorMessage();
                return Json(new{Success = false});                
            }
        }        

        #region SpecialListBooking

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmAttendancePending()
        {
            return View(DataBase.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.Confirmed }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalAppointmentsHistorial()
        {
            return View(DataBase.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.Attended, (int)AppointmentState.NotAttended, (int)AppointmentState.Rated }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult PendingConfirmToAttention()
        {
            return View(DataBase.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, new List<int>() { (int)AppointmentState.PendingConfirmation }));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult PendingConfirmToAttentionList(int? contactLocationId = null)
        {
            return View(DataBase.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId,
                new List<int>() { (int)AppointmentState.PendingConfirmation }, contactLocationId));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ConfirmToAttention(Sic.Apollo.Models.Appointment.View.ProfessionalAppointment professionalAppointment)
        {
            return View(professionalAppointment);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalNotAttended()
        {
            return View();
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
                    var appointmentTransactionsUpdate = DataBase.AppointmentTransactions.Get(p => appsUpdate.Contains(p.AppointmentTransactionId),
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
                    var appointmentTransactionsUpdate = DataBase.AppointmentTransactions.Get(p => appsUpdate.Contains(p.AppointmentTransactionId),
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


        #endregion SpecialListBooking
        
	}
}