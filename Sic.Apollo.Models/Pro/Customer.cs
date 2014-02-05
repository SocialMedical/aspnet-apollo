using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Models.General;
using System.Web.Mvc;

namespace Sic.Apollo.Models.Pro
{
    [Table("tbCustomer", Schema = "pro")]
    public class Customer : EntityBase
    {
        [Key]
        [ScaffoldColumn(false)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForID")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Contact Contact { get; set; }

        public virtual List<Appointment.AppointmentTransaction> AppointmentTransactions { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();

            this.Contact = new Contact();
            this.Contact.Gender = (int)Gender.Male;            
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            return Contact.Validate(modelState, true);
        }
    }
}