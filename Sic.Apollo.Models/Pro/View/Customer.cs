using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Data.Entity;

namespace Sic.Apollo.Models.Pro.View
{
    public class Customer : EntityBase
    {
        public int CustomerId { get; set; }
        public IEnumerable<Sic.Apollo.Models.Appointment.View.CustomerAppointment> Appointments { get; set; }
        public IEnumerable<Pro.View.Professional> CustomerProfessionals { get; set; }
    }
}
