using System.ComponentModel.DataAnnotations;
using Sic.Apollo.Resources;

namespace Sic
{
    public enum InstitutionType
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForInsurance")]
        Insurance = 1
    }
    
    public enum Gender : byte
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForMale")]
        Male = 1,
        [Display(ResourceType = typeof(Resources), Name = "LabelForFemale")]
        Female = 2,        
        NotApplicable = 3
    }

    public enum UserType
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForAdministrator")]
        Administrator = 1,
        [Display(ResourceType = typeof(Resources), Name = "LabelForProfessional")]
        Professional = 2,
        [Display(ResourceType = typeof(Resources), Name = "LabelForCustomer")]
        Customer = 3,
        [Display(ResourceType = typeof(Resources), Name = "LabelForAssistant")]
        Assistant = 4
    }

    public enum ProfessionalTeamUserType
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForAssistant")]
        Assistant = 4
    }

    public enum UserState
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForPendingConfirmation")]
        PendingConfirmation = 1,
        [Display(ResourceType = typeof(Resources), Name = "LabelForActive")]
        Active = 2,
        [Display(ResourceType = typeof(Resources), Name = "LabelForSuspended")]
        Suspended = 3,
        [Display(ResourceType = typeof(Resources), Name = "LabelForInactive")]
        Inactive = 4
    }

    public enum AppointmentState
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForPending")]
        Pending = 1,
        [Display(ResourceType = typeof(Resources), Name = "LabelForPendingConfirmation")]
        PendingConfirmation = 2,
        [Display(ResourceType = typeof(Resources), Name = "LabelForConfirmed")]
        Confirmed = 3,
        [Display(ResourceType = typeof(Resources), Name = "LabelForCanceled")]
        Canceled = 4,
        [Display(ResourceType = typeof(Resources), Name = "LabelForAttended")]
        Attended = 5,
        [Display(ResourceType = typeof(Resources), Name = "LabelForNotAttended")]
        NotAttended = 6,
        [Display(ResourceType = typeof(Resources), Name = "LabelForRated")]
        Rated = 7,
        [Display(ResourceType = typeof(Resources), Name = "LabelForAppointmentRescheduled")]
        ReSchedule = 8,
        [Display(ResourceType = typeof(Resources), Name = "LabelForObsoleteAppointment")]
        Obsolete = 9
    }

    public enum PatientState
    {   
        Inactive = 0,
        Active = 1
    }

    public enum BloodGroup
    {                
        [Display(ResourceType = typeof(Resources), Name = "LabelForUnspecified")]
        Unspecified = 0,
        [Display(Name="O-")]
        OLess = 1,
        [Display(Name = "O+")]
        OPLus = 2,
        [Display(Name = "A-")]
        ALess = 3,
        [Display(Name = "A+")]
        APlus = 4,
        [Display(Name = "B-")]
        BLess = 5,
        [Display(Name = "B+")]
        BPlus = 6,
        [Display(Name = "AB-")]
        ABLess = 7,
        [Display(Name = "AB+")]
        ABPlus = 8
    }

    //public enum VitalSign
    //{
    //    Weight,
    //    Height,
    //    RectalTemperature,
    //    AxillaryTemperature,
    //    BloodPressure,
    //    HeartRate,
    //    RespiratoryRate
    //}

    public enum MedicalProblemType
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForFamilyProblem")]
        Family = 1,
        [Display(ResourceType = typeof(Resources), Name = "LabelForPathologicalProblem")]
        Pathological = 2,
        [Display(ResourceType = typeof(Resources), Name = "LabelForNoPathologicalProblem")]
        NoPathological = 3,
        [Display(ResourceType = typeof(Resources), Name = "LabelForObstetricGynecologyProblem")]
        ObstetricGynecology = 4
    }

    public enum MeasurementUnit
    {
        [Display(ResourceType = typeof(Resources), Name = "LabelForKilogramSymbol")]
        Kilogram = 10,

        [Display(ResourceType = typeof(Resources), Name = "LabelForCentimenterSymbol")]
        Centimeter = 20,

        [Display(ResourceType = typeof(Resources), Name = "LabelForCelsiusSymbol")]
        Celsius = 30,

        [Display(ResourceType = typeof(Resources), Name = "LabelForMinuteSymbol")]
        Minute = 40,

        [Display(ResourceType = typeof(Resources), Name = "LabelForMercuryMillimetersSymbol")]
        MercuryMillimeters = 50,

        [Display(ResourceType = typeof(Resources), Name = "LabelForBeatsPerMinuteSymbol")]
        BeatsPerMinute = 60, //Latidos por Minuto

        [Display(ResourceType = typeof(Resources), Name = "LabelForBreathsPerMinuteSymbol")]
        BreathsPerMinute = 70
    }
}