using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sic.Apollo.Models.Appointment.View
{
    public class AppointmentRate
    {
        public long AppointmentId { get; set; }
        public long AppointmentTransactionId { get; set; }

        public int? CustomerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public int? RateScore1 { get; set; }
        public int? RateScore2 { get; set; }
        public int? RateScore3 { get; set; }
        public string RateComments { get; set; }
        public DateTime? RateDate { get; set; }

        public string Description
        {
            get 
            {
                return String.Format("{0:d} {1} {2}", RateDate, Sic.Apollo.Resources.Resources.LabelForBy, FullName);
            }
        }

        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(LastName))
                    return LastName + ", " + FirstName;
                else
                    return FirstName;
            }
        }
    }
}