using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models.Pro.View
{
    public class ProfessionalSummary: Models.Pro.View.Professional
    {
        public int CustomerCount { get; set; }

        public int AppointmentPendingCount { get; set; }

        public int AppointmentPendingConfirmationToAttentionCount { get; set; }

        public int AppointmentPendingCheckAttentionCount { get; set; }

        public int AppointmentCount { get; set; }

        public int CommentsCount { get; set; }

        public int OfficeCount { get; set; }

        public int PatientCount { get; set; }
    }
}
