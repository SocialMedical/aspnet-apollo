using Sic.Apollo.Areas.Public.Controllers;
using Sic.Apollo.Models;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Appointment.Controllers
{
    public class ProfessionalController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        #region ProfessionalAppointment

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

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalBook()
        {
            var schedules = DataBase.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);

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
            return View(DataBase.ProfessionalOffices.Get(p => p.ContactId == Sic.Apollo.Session.ProfessionalId));
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerBook()
        {
            HomeController.FillSearchCriteria(this, DataBase);

            var viewModel = new Models.Pro.View.Customer()
            {
                CustomerId = Sic.Web.Mvc.Session.UserId
            };

            viewModel.Appointments = DataBase.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId);
            viewModel.CustomerProfessionals = DataBase.Customers.GetCustomerProfessionals(Sic.Web.Mvc.Session.UserId);

            return View(viewModel);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult ProfessionalBookDetails(int contactLocationId, long startDate, int duration, long startTime, long endTime)
        {
            ViewBag.DaysModel = Utils.DayOfWeek(startDate);

            DateTime startDateTime = new DateTime(startDate);
            DateTime endDateTime = startDateTime.AddDays(7);

            ViewBag.Duration = duration;
            ViewBag.StartTime = new DateTime(startTime);
            ViewBag.EndTime = new DateTime(endTime);

            ViewBag.ContactLocationId = contactLocationId;
            ViewBag.StartDate = startDateTime.Date;
            ViewBag.EndDate = endDateTime;

            var schedules = DataBase.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == Sic.Apollo.Session.ProfessionalId);
            ViewBag.Schedules = schedules;

            return PartialView(DataBase.AppointmentTransactions.GetProfessionalAppointments(Sic.Apollo.Session.ProfessionalId, contactLocationId == 0 ? (int?)null : contactLocationId,
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
            return PartialView(viewName, DataBase.AppointmentTransactions.GetProfessionalAppointments(appointmentTransactionId: appointmentTransactionId).SingleOrDefault());
        }

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

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
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
                DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId,
                    includeProperties: "Appointment,Appointment.Professional.Contact,Customer.Contact,Appointment.ProfessionalOffice").Single();

                CancelAppointmentToAttention(appointmentTransactionUpdate, confirmToAttentionNotes);

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentTransactionUpdate.Appointment.ProfessionalId),
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
                var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
                appointmentTransactionUpdate.State = (int)AppointmentState.Attended;
                appointmentTransactionUpdate.AttentionNotes = attentionNotes;

                var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
                appointmentUpdate.State = (int)AppointmentState.Attended;

                DataBase.Appointments.Update(appointmentUpdate);
                DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

                DataBase.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        //PatientCount = DataBase.Professionals.GetPatientCount(appointment.ProfessionalId),
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId),
                        Message = string.Format(Sic.Apollo.Resources.Resources.MessageForConfirmAttended,
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

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        AppointmentPendingCount = DataBase.Professionals.GetAppointmentPendingCount(appointmentUpdate.ProfessionalId),
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
	}
}