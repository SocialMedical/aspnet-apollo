using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using Sic.Web.Mvc;
using System.Text;

namespace Sic.Apollo.Models.Pro 
{
    [Table("tbProfessionalOfficeSchedule", Schema = "pro")]
    public class ProfessionalOfficeSchedule : EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int ProfessionalOfficeScheduleId { get; set; }

        public int ContactLocationId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForSunday")]     
        public bool Sunday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForMonday")]     
        public bool Monday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForTuesday")]     
        public bool Tuesday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForWednesday")]     
        public bool Wednesday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForThursday")]     
        public bool Thursday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForFriday")]     
        public bool Friday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForSaturday")]     
        public bool Saturday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForAppointmentDuration")]     
        public short AppointmentDuration { get; set; }

        [Required]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleStartTime")]     
        public DateTime StartTime { get; set; }
        
        [Required]
        [CompareValues("StartTime", CompareValues.GreaterThan, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldComparer")]        
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleEndTime")]        
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1,48)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForEach")]     
        public byte ForEachWeek { get; set; }

        [NotMapped]
        [Required]        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Sic.Core.Constants.FormatString.DefaultEditorDateFormat)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleStartDate")]
        public string ValidityStartDateString { get; set; }        

        [Required]             
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Sic.Core.Constants.FormatString.DefaultEditorDateFormat)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleStartDate")]
        public DateTime ValidityStartDate { get; set; }
        
        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [CompareValues("ValidityStartDateString", CompareValues.GreatThanOrEqualTo, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldComparer")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleEndDate")]
        public string ValidityEndDateString { get; set; }

        [DataType(DataType.Date)]        
        [DisplayFormat(ApplyFormatInEditMode=true,DataFormatString="{0:dd/MM/yyyy}")]
        [CompareValues("ValidityStartDate", CompareValues.GreatThanOrEqualTo, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "ValidationFieldComparer")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleEndDate")]
        public DateTime? ValidityEndDate { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForScheduleEndTime")]
        public bool IndefiniteEndDate { get; set; }

        [NotMapped]
        public long StartTimeTicks
        {
            get
            {
                return this.StartTime.Ticks;
            }
            set
            {
                this.StartTime = new DateTime(value);
            }       
        }

        [NotMapped]
        public long EndTimeTicks
        {
            get
            {
                return this.EndTime.Ticks;
            }
            set
            {
                this.EndTime = new DateTime(value);
            }
        }

        [NotMapped]
        public long ValidityStartDateTicks
        {
            get
            {
                return this.ValidityStartDate.Ticks;
            }
            set
            {
                this.ValidityStartDate = new DateTime(value);
            }
        }

        [NotMapped]
        public long? ValidityEndDateTicks
        {
            get
            {
                if (this.ValidityEndDate.HasValue)
                    return this.ValidityEndDate.Value.Ticks;
                else
                    return null;
            }
            set
            {
                if (!value.HasValue)
                    this.ValidityEndDate = null;
                else
                    this.ValidityEndDate = new DateTime(value.Value);
            }
        }

        [Required]        
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "LabelForDescription")]
        public string Description
        {
            get
            {
                StringBuilder daysOfWeek = new StringBuilder();
                if (this.Monday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForMonday + ", ");
                if (this.Tuesday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForTuesday + ", ");
                if (this.Wednesday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForWednesday + ", ");
                if (this.Thursday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForThursday + ", ");
                if (this.Friday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForFriday + ", ");
                if (this.Saturday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForSaturday + ", ");
                if (this.Sunday) daysOfWeek.Append(Sic.Apollo.Resources.Resources.LabelForSunday + ", ");

                if (daysOfWeek.Length > 2)
                {
                    daysOfWeek.Remove(daysOfWeek.Length - 2, 1);
                }
                //Ocurre el(los) día(s) {0} de {1:t} a {2:t} de cada {3} semana(s). Empezando el {4:dd/MMM/yyyy} {5} {6:dd/MMM/yyyy}
                string description = String.Format(Sic.Apollo.Resources.Resources.LegendForScheduleDescription,
                    daysOfWeek, this.StartTime, this.EndTime, this.ForEachWeek,
                    this.ValidityStartDate,
                    this.IndefiniteEndDate ? "" : Sic.Apollo.Resources.Resources.LabelForUntil,
                    this.IndefiniteEndDate ? null : this.ValidityEndDate);

                return description;
            }
        }

        public virtual ProfessionalOffice ProfessionalOffice { get; set; }
        public virtual List<Appointment.Appointment> Appointments { get; set; }
    }
}