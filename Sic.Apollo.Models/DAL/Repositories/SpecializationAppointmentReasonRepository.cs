using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Sic.Apollo.Models.Appointment;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Repositories
{
    public class SpecializationAppointmentReasonRepository: Sic.Data.Entity.Repository<Sic.Apollo.Models.Appointment.SpecializationAppointmentReason>
    {
        public SpecializationAppointmentReasonRepository(Sic.Data.Entity.DbContext context)
            :base(context)
        {
        }

        //public IEnumerable<SpecializationAppointmentReason> GetSpecializationAppointmentReason(int appointmentId)
        //{
        //    Context db = (Context)context;

        //    return from a in db.Appointments
        //           join p in db.ProfessionalSpecializations on a.ProfessionalId equals p.ProfessionalId
        //           join r in db.SpecializationAppointmentReasons on p.SpecializationId equals r.SpecializationId
        //           where a.AppointmentId == appointmentId
        //           select r;
        //}
    }
}