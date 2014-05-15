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
using Sic.Apollo.Areas.Public.Controllers;

namespace Sic.Apollo.Areas.Appointment.Controllers
{
    public class CustomerController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {                
        #region CustomerAppointment

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

        [Authorize(UserType.Customer)]
        public ActionResult CustomerMyAppointments()
        {
            return View(DataBase.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId));
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerPendingRatings()
        {
            return View(DataBase.AppointmentTransactions.GetCustomerAppointments(Sic.Web.Mvc.Session.UserId, new List<int>() { (int)AppointmentState.Attended }));
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
            var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
            appointmentTransactionUpdate.State = (int)AppointmentState.Rated;

            appointmentTransactionUpdate.RateScore1 = rateScore1;
            appointmentTransactionUpdate.RateScore2 = rateScore2;
            appointmentTransactionUpdate.RateScore3 = rateScore3;
            appointmentTransactionUpdate.RateComments = rateComments;
            appointmentTransactionUpdate.RateDate = Sic.Web.Mvc.Session.CurrentDateTime;

            var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Rated;

            var score = DataBase.Professionals.GetScore(appointmentUpdate.ProfessionalId);

            if (score != null)
            {
                score.CountRateScore++;

                if (score.CountRateScore >= 3)
                {
                    var professional = DataBase.Professionals.Get(p => p.ProfessionalId == appointmentUpdate.ProfessionalId).Single();

                    professional.RateScore1 = (score.SumRateScore1 + rateScore1) / score.CountRateScore;
                    professional.RateScore2 = (score.SumRateScore2 + rateScore2) / score.CountRateScore;
                    professional.RateScore3 = (score.SumRateScore3 + rateScore3) / score.CountRateScore;

                    professional.RateScore = (professional.RateScore1 + professional.RateScore2 + professional.RateScore3) / 3;

                    DataBase.Professionals.Update(professional);
                }
            }

            DataBase.Appointments.Update(appointmentUpdate);
            DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

            DataBase.Save();

            var contact = DataBase.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();
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
            var appointmentTransactionUpdate = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == appointmentTransactionId).Single();
            appointmentTransactionUpdate.State = (int)AppointmentState.Canceled;

            var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransactionUpdate.AppointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.Pending;

            DataBase.Appointments.Update(appointmentUpdate);
            DataBase.AppointmentTransactions.Update(appointmentTransactionUpdate);

            DataBase.Save();

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
            var appointment = DataBase.Appointments.Get(p => p.AppointmentId == appointmentId).Single();

            ViewBag.ReschudeleAppointmentTransactionId = reschudeleAppointmentTransactionId;

            return View(appointment);
        }

        [Authorize(UserType.Customer)]
        public ActionResult CustomerRescheduleBook(long appointmentId, long reschudeleAppointmentTransactionId)
        {
            var appointmentUpdate = DataBase.Appointments.Get(p => p.AppointmentId == appointmentId).Single();
            appointmentUpdate.State = (int)AppointmentState.PendingConfirmation;

            var appointmentTransaction = DataBase.AppointmentTransactions.Get(p => p.AppointmentTransactionId == reschudeleAppointmentTransactionId).Single();
            appointmentTransaction.State = (int)AppointmentState.Canceled;
            appointmentTransaction.CancelTransactionDate = Sic.Web.Mvc.Session.CurrentDateTime;

            var appointmentUpdateCancel = DataBase.Appointments.Get(p => p.AppointmentId == appointmentTransaction.AppointmentId).Single();
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

            DataBase.Appointments.Update(appointmentUpdate);
            DataBase.Appointments.Update(appointmentUpdateCancel);
            DataBase.AppointmentTransactions.Update(appointmentTransaction);
            DataBase.AppointmentTransactions.Insert(appointmentTransactionInsert);

            DataBase.Save();

            var contact = DataBase.Contacts.Get(p => p.ContactId == appointmentUpdate.ProfessionalId).FirstOrDefault();

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

        

        
    }
}
