using Sic.Apollo.Models;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Appointment.Controllers
{
    public class ScheduleController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        #region Schedule

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult Preview(int contactLocationId, string weekDays = "", long? startTimeOfDay = null, long? endTimeOfDay = null,
            int? appointmentDuration = 30, long? startDate = null, long? endDate = null,
            int eachWeek = 1, long? startConfiguration = null, int? professionalOfficeScheduleId = null, int visibleDays = 7)
        {
            ViewBag.DaysModel = Utils.DayOfWeek(startDate);

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

                var result = DataBase.Appointments.GetPreviewAppointments(contactLocationId,
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