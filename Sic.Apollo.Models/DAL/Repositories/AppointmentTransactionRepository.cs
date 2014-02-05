using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using Sic.Apollo.Models.Appointment.View;

namespace Sic.Apollo.Models.Repositories
{
    public class AppointmentTransactionRepository : Repository<Appointment.AppointmentTransaction>
    {
        public AppointmentTransactionRepository(Sic.Data.Entity.DbContext context)
            :base(context)
        {
        }

        public bool VerifyMaxAppointmentsOpen(DateTime startDate)
        {
            DateTime beginDate = startDate.Date;
            DateTime endDate = beginDate.AddDays(1).AddMilliseconds(-1);
            Context db = (Context)context;

            return db.AppointmentTransactions.Where(p =>
                p.CustomerId == Sic.Web.Mvc.Session.UserId
                && p.Appointment.StartDate >= beginDate && p.Appointment.EndDate <= endDate
                && (p.State == (byte)AppointmentState.PendingConfirmation
                    || p.State == (byte)AppointmentState.Confirmed)).Count() >= 3;
        }

        public List<CustomerAppointment> GetCustomerAppointments(int CustomerId, List<int> appointmentState = null)
        {
            Context db = (Context)context;
            
            var appointments = (from p in db.AppointmentTransactions
                                join r in db.SpecializationAppointmentReasons on p.SpecializationAppointmentReasonId equals r.SpecializationAppointmentReasonId
                                join a in db.Appointments on p.AppointmentId equals a.AppointmentId    
                                join i in db.InsuranceInstitutions on p.InsuranceInstitutionId equals i.InstitutionId into li
                                from ii in li.DefaultIfEmpty()
                                where p.CustomerId == CustomerId
                                select new CustomerAppointment
                                {
                                    AppointmentId = p.AppointmentId,
                                    AppointmentTransactionId = p.AppointmentTransactionId,
                                    ProfessionalId = a.ProfessionalId,
                                    ContactLocationId = a.ContactLocationId,
                                    LastName = a.Professional.Contact.LastName,
                                    FirstName = a.Professional.Contact.FirstName,
                                    StartDate = a.StartDate,
                                    ReasonForVisit = r.Name,
                                    State = p.State,
                                    InsuranceInstitution = ii.Contact.FirstName,
                                    Picture = a.Professional.Contact.Picture,
                                    SpecializationAppointmentReasonId = p.SpecializationAppointmentReasonId,
                                    SpecializationId = r.SpecializationId
                                }).ToList();


            if(appointmentState != null)
                appointments = appointments.Where(p=> appointmentState.Contains(p.State)).ToList();

            return appointments;
        }

        public List<ProfessionalAppointment> GetProfessionalAppointments(int? professionalId = null, int? contactLocation = null, DateTime? startDate = null, DateTime? endDate = null,List<int> appointmentState = null,
            int? appointmentTransactionId = null)
        {
            Context db = (Context)context;            

            var query = from p in db.AppointmentTransactions
                                join a in db.Appointments on p.AppointmentId equals a.AppointmentId
                                join i in db.InsuranceInstitutions on p.InsuranceInstitutionId equals i.InstitutionId into li
                                from ii in li.DefaultIfEmpty()                                
                                select new ProfessionalAppointment
                                {                        
                                    ProfessionalId = a.ProfessionalId,
                                    ProfessionalOfficeId = a.ContactLocationId,
                                    AppointmentId = p.AppointmentId,
                                    AppointmentTransactionId = p.AppointmentTransactionId,
                                    CustomerId = p.CustomerId,
                                    LastName = p.Customer.Contact.LastName,
                                    FirstName = p.Customer.Contact.FirstName,
                                    StartDate = a.StartDate,
                                    EndDate = a.EndDate,
                                    CustomerNameReference = p.CustomerNameReference,
                                    State = p.State,
                                    OfficeAddress = a.ProfessionalOffice.Address,                                    
                                    ReasonForVisit = p.SpecializationAppointmentReason.Name,
                                    InsuranceInstitution = ii.Contact.FirstName,
                                    ContactPhoneNumber = p.ContactPhoneNumber,
                                    CustomerNotes = p.CustomerNotes,
                                    IsCustomerAppointement = p.AppointmentForMe
                                };                                

            if(appointmentTransactionId.HasValue)
                query = query.Where(a => a.AppointmentTransactionId == appointmentTransactionId.Value);            

            if (professionalId.HasValue)
                query = query.Where(a => a.ProfessionalId == professionalId.Value);            

            if (startDate.HasValue && endDate.HasValue)
            {
                startDate = startDate.Value.Date;
                endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

                query = query.Where(a => a.StartDate >= startDate && a.StartDate <= endDate);
            }

            if (appointmentState != null)
                query = query.Where(a=>appointmentState.Contains(a.State));

            if (contactLocation != null && contactLocation!=0)
                query = query.Where(p => p.ProfessionalOfficeId == contactLocation);

            return query.ToList();
        }

        public List<ProfessionalOfficeAppointment> GetProfessionalAppointments(int professionalId, List<int> appointmentState,
            int? contactLocationId = null)
        {
            Context db = (Context)context;

            var queryAppointments = (from p in db.AppointmentTransactions
                                join a in db.Appointments on p.AppointmentId equals a.AppointmentId
                                join i in db.InsuranceInstitutions on p.InsuranceInstitutionId equals i.InstitutionId into li
                                from ii in li.DefaultIfEmpty()
                                where a.ProfessionalId == professionalId
                                && appointmentState.Contains(a.State)
                                select new ProfessionalAppointment
                                {
                                    ProfessionalOfficeId = a.ContactLocationId,
                                    AppointmentId = p.AppointmentId,
                                    AppointmentTransactionId = p.AppointmentTransactionId,
                                    CustomerId = p.CustomerId,
                                    LastName = p.Customer.Contact.LastName,
                                    FirstName = p.Customer.Contact.FirstName,
                                    StartDate = a.StartDate,
                                    State = p.State,
                                    ReasonForVisit = p.SpecializationAppointmentReason.Name,
                                    InsuranceInstitution = ii.Contact.FirstName,
                                    ContactPhoneNumber = p.ContactPhoneNumber,
                                    CustomerNotes = p.CustomerNotes,
                                    ContactLocationId = a.ContactLocationId
                                });

            if (contactLocationId.HasValue)
                queryAppointments.Where(p => p.ContactLocationId == contactLocationId);

            var appointments = queryAppointments.ToList();

            var queryOffice = (from p in db.ContactLocations
                           join a in db.ProfessionalOffices on new { ProfessionalId = p.ContactId, p.ContactLocationId } equals new { a.ProfessionalId, a.ContactLocationId }
                           where p.ContactId == professionalId
                                   select new
                           {
                               ProfessionalOfficeId = p.ContactLocationId,
                               p.Description,
                               p.ContactLocationId
                           });

            if (contactLocationId.HasValue)
                queryOffice = queryOffice.Where(p => p.ContactLocationId == contactLocationId);

            var offices = queryOffice.ToList();
                          
            List<ProfessionalOfficeAppointment> list = new List<ProfessionalOfficeAppointment>();

            foreach (var item in offices)
            {
                var office = new ProfessionalOfficeAppointment()
                {
                    ProfessionalOfficeId = item.ProfessionalOfficeId,
                    Description = item.Description,
                    ProfessionalAppointments = new List<ProfessionalAppointment>()
                };

                list.Add(office);
            }

            foreach (var item in appointments)
            {
                var office = list.Where(p=>p.ProfessionalOfficeId == item.ProfessionalOfficeId).FirstOrDefault();

                if (office != null)
                    office.ProfessionalAppointments.Add(item);                        
            }

            return list;
        }

        public List<AppointmentRate> GetRateAppointments(int ProfessionalId, bool resume)
        {
            Context db = (Context)context;

            var query = (from p in db.AppointmentTransactions
                    join a in db.Appointments on p.AppointmentId equals a.AppointmentId
                    join c in db.Contacts on p.CustomerId equals c.ContactId
                    where a.ProfessionalId == ProfessionalId && p.State == (int)AppointmentState.Rated
                    select new AppointmentRate
                    {
                        AppointmentId = p.AppointmentId,
                        AppointmentTransactionId = p.AppointmentTransactionId,                                    
                        LastName = c.LastName,
                        FirstName = c.FirstName,
                        CustomerId = p.CustomerId,
                        RateScore1 = p.RateScore1,
                        RateScore2 = p.RateScore2,
                        RateScore3 = p.RateScore3,
                        RateDate = p.RateDate,
                        RateComments = p.RateComments
                    }).OrderByDescending(p=>p.RateDate);

            if (resume)
                return query.Take(3).ToList();
            else
                return query.Skip(3).ToList();
        }
    }
}