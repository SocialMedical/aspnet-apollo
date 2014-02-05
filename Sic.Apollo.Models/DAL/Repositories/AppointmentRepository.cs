using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.Data.Entity;
using Sic.Apollo.Models.Appointment.View;
using Sic.Apollo.Models.Pro;
using System.Text;
using Sic.Data;
using System.Globalization;

namespace Sic.Apollo.Models.Repositories
{
    public class AppointmentRepository : Repository<Appointment.Appointment>
    {
        public AppointmentRepository(Sic.Data.Entity.DbContext context)
            :base(context)
        {
        }

        public void DeleteScheduleAppointment(int professionalOfficeScheduleId)
        {
            context.PendingCommands.Add(string.Format(QueryDeleteAppointmentSchedule, professionalOfficeScheduleId,
                (int)AppointmentState.Pending, (int)AppointmentState.Obsolete));
        }

        public bool GenerateAppointments(ProfessionalOfficeSchedule schedule)
        {            
            StringBuilder days = new StringBuilder();
            if(schedule.Monday) days.Append("1|");
            if(schedule.Tuesday) days.Append("2|");
            if(schedule.Wednesday) days.Append("3|");
            if(schedule.Thursday) days.Append("4|");
            if(schedule.Friday) days.Append("5|");
            if(schedule.Saturday) days.Append("6|");
            if(schedule.Sunday) days.Append("0|");            

            DateTime finish = schedule.ValidityStartDate.AddDays(30);
            if(!schedule.IndefiniteEndDate && schedule.ValidityEndDate.HasValue && schedule.ValidityEndDate.Value < finish)
            {
                finish = schedule.ValidityEndDate.Value;
            }
            var entries = GetPreviewAppointments(schedule.ContactLocationId,days.ToString(),schedule.StartTime,
                schedule.EndTime, schedule.AppointmentDuration, schedule.ValidityStartDate, finish, schedule.ForEachWeek, schedule.ValidityStartDate);
            

            ProfessionalOffice office = null;
            if (schedule.ProfessionalOffice != null)
                office = schedule.ProfessionalOffice;
            else
            {
                office = ((Context)context).ProfessionalOffices.Single(p=>p.ContactLocationId == schedule.ContactLocationId);
            }

            if (schedule.ProfessionalOfficeScheduleId != 0)
            {
                var appExists = ((Context)context).Appointments.Where(p => p.ProfessionalOfficeScheduleId == schedule.ProfessionalOfficeScheduleId
                    && !p.AppointmentTransactions.Any());

                HashSet<DateTime> hsApp = new HashSet<DateTime>(entries.AppointmentEntries.Select(p => p.StartDate));
                foreach (var app in appExists)
                {
                    if (hsApp.Contains(app.StartDate))
                    {
                        entries.AppointmentEntries.RemoveAll(p => p.StartDate == app.StartDate);//Remove exists app- no insert
                    }
                    else
                    {
                        this.Delete(app);
                    }
                }
            }

            foreach (var app in entries.AppointmentEntries)
            {               
                Appointment.Appointment appointmentInsert = new Appointment.Appointment();
                appointmentInsert.ProfessionalOfficeScheduleId = schedule.ProfessionalOfficeScheduleId;
                appointmentInsert.ContactLocationId = schedule.ContactLocationId;
                appointmentInsert.ProfessionalOffice = office;
                appointmentInsert.EndDate = app.StartDate.AddMinutes(schedule.AppointmentDuration);
                appointmentInsert.StartDate = app.StartDate;
                appointmentInsert.State = (int)AppointmentState.Pending;
                appointmentInsert.ProfessionalId = office.ProfessionalId;

                this.Insert(appointmentInsert);
            }
            
            return true;
        }

        public ContactLocationAppointments GetPreviewAppointments(int contactLocationId, string weekDays, DateTime startTimeOfDay, DateTime endTimeOfDay,
            int appointmentDuration, DateTime start, DateTime end, int eachWeek,
            DateTime startConfigurationDate,
            int? professionalOfficeScheduleId = null)
        {
            Models.Appointment.View.ContactLocationAppointments app = new ContactLocationAppointments();
            app.AppointmentEntries = new List<AppointmentEntry>();
            app.ContactLocationId = contactLocationId;

            int[] previewWeekDays = weekDays.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToArray();

            DateTime currentDateTime = Sic.Web.Mvc.Session.CurrentDateTime;

            DateTime startTime = startTimeOfDay;
            DateTime endTime = endTimeOfDay;

            DateTime currentDate = startConfigurationDate.Date;
            DateTime? lastApointmentDate = null;

            while (currentDate <= end)
            {                
                if (previewWeekDays.Contains((int)currentDate.DayOfWeek))
                {
                    if (!lastApointmentDate.HasValue ||
                        Sic.Core.Globalization.Calendar.DiffWeeks(lastApointmentDate.Value.Date, currentDate.Date, System.DayOfWeek.Monday) % eachWeek == 0)
                    {
                        lastApointmentDate = currentDate.Date;

                        if (currentDate >= start.Date)
                        {
                            DateTime currenTime = startTime;
                            while (currenTime < endTime)
                            {
                                if (!(currentDateTime.Date == currentDate.Date && currenTime.TimeOfDay < currentDateTime.TimeOfDay))
                                {
                                    Models.Appointment.View.AppointmentEntry entry = new AppointmentEntry();
                                    entry.ContactLocationId = app.ContactLocationId;
                                    entry.StartDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currenTime.Hour, currenTime.Minute, 0);
                                    app.AppointmentEntries.Add(entry);
                                }
                                currenTime = currenTime.AddMinutes(appointmentDuration);
                            }
                        }
                    }
                }
             
                currentDate = currentDate.AddDays(1);
            }

            return app;
        }

        public List<ContactLocationAppointments> GetAppointments(
            int[] locationsId, DateTime? startDate, int numberDaysAppointemt=7)
        {            
            Context db = (Context)context;

            if (startDate == null)
                startDate = Sic.Web.Mvc.Session.CurrentDateTime;

            DateTime endDate = startDate.Value.Date.AddDays(numberDaysAppointemt).AddSeconds(-1);

            var appointments =  (from p in db.Appointments
                                where locationsId.Contains(p.ContactLocationId)
                                && p.StartDate >= startDate.Value && p.StartDate <= endDate
                                && p.State == (byte)AppointmentState.Pending
                                select new
                                {
                                    p.AppointmentId,
                                    p.ContactLocationId,
                                    p.StartDate,
                                }).ToList();

            List<ContactLocationAppointments> list = new List<ContactLocationAppointments>();
            
            foreach (int locationId in locationsId)
            {
                List<AppointmentEntry> contactLocationAppointments = new List<AppointmentEntry>();

                var listAppointments = appointments.Where(p => p.ContactLocationId == locationId).ToList();

                foreach (var ap in listAppointments)
                    contactLocationAppointments.Add(new AppointmentEntry()
                    {   
                        AppointmentId = ap.AppointmentId,
                        ContactLocationId = ap.ContactLocationId,
                        StartDate = ap.StartDate
                    });
                
                list.Add(new Appointment.View.ContactLocationAppointments()
                    {
                        ContactLocationId = locationId,                        
                        AppointmentEntries = contactLocationAppointments
                    });
            }            

            return list;
        }

        #region QueryString

        string QueryDeleteAppointmentSchedule = 
                            "DELETE apo.tbAppointment " +
                            "WHERE ProfessionalOfficeScheduleId = {0} " +
                            "AND (State = {1} OR State = {2}) " +
                            "UPDATE apo.tbAppointment SET ProfessionalOfficeScheduleId = NULL " +
                            "WHERE ProfessionalOfficeScheduleId = {0}";

        string QueryInsert= "INSERT INTO apo.tbAppointment (" +
                            "ProfessionalId," +
                            "ProfessionalOfficeScheduleId," +
            	            "ContactLocationId," +
	                        "StartDate," +
	                        "EndDate," +
	                        "State" +
                            ") VALUES ( " +
                            "@ProfessionalId," +
                            "@ProfessionalOfficeScheduleId," +
            	            "@ContactLocationId," +
	                        "@StartDate," +
	                        "@EndDate," +
	                        "@State)";

        #endregion      
          
        public override void Insert(Appointment.Appointment entity)
        {
            context.PendingCommands.Add(Parameter.ApplyParameters(QueryInsert,
                entity.GetParameters(new string[] { "AppointmentId" })));
        }
        
    }
}