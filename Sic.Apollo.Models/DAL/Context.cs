using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models.Appointment;
using Sic.Apollo.Models.General;
using Sic.Apollo.Models.Security;
using Sic.Apollo.Models.Medical;

namespace Sic.Apollo.Models
{
    public class Context: Sic.Data.Entity.DbContext
    {
        public Context(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Professional> Professionals { get; set; }
        public DbSet<ProfessionalOffice> ProfessionalOffices { get; set; }
        public DbSet<ProfessionalSchool> ProfessionalSchools { get; set; }
        public DbSet<ProfessionalExperience> ProfessionalExperiences { get; set; }
        public DbSet<ProfessionalVademecum> ProfessionalVademecums { get; set; }
        public DbSet<ProfessionalTeam> ProfessionalTeams { get; set; }
        public DbSet<Vademecum> Vademecums { get; set; }

        public DbSet<Appointment.Appointment> Appointments { get; set; }
        public DbSet<Appointment.AppointmentTransaction> AppointmentTransactions { get; set; }
        public DbSet<ProfessionalOfficeSchedule> ProfessionalOfficeSchedules { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerProfessional> CustomerProfessionals { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactLocation> ContactLocations { get; set; }
        public DbSet<ContactLocationPicture> ContactLocationPictures { get; set; }

        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<SpecializationAppointmentReason> SpecializationAppointmentReasons { get; set; }

        public DbSet<Institution> Institutions { get; set; }
        public DbSet<InsuranceInstitution> InsuranceInstitutions { get; set; }
        public DbSet<ProfessionalSpecialization> ProfessionalSpecializations { get; set; }
        
        public DbSet<InsuranceInstitutionPlan> InsuranceInstitutionPlans { get; set; }
        public DbSet<ProfessionalInsuranceInstitutionPlan> ProfessionalInsuranceInstitutionPlans { get; set; }
        
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Area> Areas { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<ProfessionalPatient> ProfessionalPatients { get; set; }
        public DbSet<MedicalCare> MedicalHistories { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }
        public DbSet<PhysicalExamination> PhysicalExaminations { get; set; }
        public DbSet<PatientPhysicalExamination> PatientPhysicalExaminations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {                      
            #region User - Contact

            modelBuilder.Entity<User>()
                .HasKey(x => new { x.UserId });

            // Extended To Main 1 on 1
            modelBuilder.Entity<User>()
                .HasRequired(i => i.Contact)
                .WithRequiredDependent();

            #endregion

            #region Professional            

            // Extended To Main 1 on 1
            modelBuilder.Entity<Professional>()
                .HasRequired(i => i.Contact)
                .WithRequiredDependent();            

            modelBuilder.Entity<Professional>().
                HasMany(p => p.ProfessionalSpecializations).WithRequired(p => p.Professional);

            modelBuilder.Entity<Professional>().HasMany(p => p.ProfessionalExperiences).WithRequired();
        
            modelBuilder.Entity<Professional>().
                HasMany(p => p.ProfessionalSchools).WithRequired();

            modelBuilder.Entity<Professional>().
                HasMany(p => p.ProfessionalCommunities).WithRequired();

            modelBuilder.Entity<Specialization>().
                HasMany(p => p.ProfessionalSpecializations).WithRequired(p => p.Specialization);

            modelBuilder.Entity<Professional>().HasMany(p => p.ProfessionalOffices).
                WithRequired(p => p.Professional);

            modelBuilder.Entity<Professional>().HasMany(p => p.ProfessionalVademecums).
                WithRequired(p => p.Professional);

            modelBuilder.Entity<ProfessionalSchool>().
                HasOptional(p => p.Institution).WithMany(p=>p.ProfessionalSchools);

            modelBuilder.Entity<ProfessionalCommunity>().
                HasOptional(p => p.Institution).WithMany(p=>p.ProfessionalCommunities);

            modelBuilder.Entity<ProfessionalExperience>().
                HasOptional(p => p.Institution).WithMany(p=>p.ProfessionalExperiences);

            //Optional
            modelBuilder.Entity<ProfessionalSpecialization>().
                HasRequired(c => c.Professional).WithMany();

            modelBuilder.Entity<ProfessionalSpecialization>().
                HasRequired(c => c.Specialization).WithMany();

            modelBuilder.Entity<ProfessionalSpecialization>().
                HasMany(p => p.ProfessionalSpecializationServices).WithOptional();

            modelBuilder.Entity<Specialization>().
                HasMany(p => p.SpecializationServices).WithRequired();

            modelBuilder.Entity<ProfessionalSpecializationService>().
                HasOptional(p => p.SpecializationService).WithRequired();           

            //modelBuilder.Entity<Specialization>().
            //    HasMany(c => c.Professionals).
            //    WithMany(p => p.Specializations).
            //    Map(
            //        m =>
            //        {
            //            m.MapLeftKey("SpecializationId");
            //            m.MapRightKey("ProfessionalId");
            //            m.ToTable("tbProfessionalSpecialization","pro");
            //        });    

            #endregion            

            #region ProfessionalTeam

            modelBuilder.Entity<Professional>().HasMany(p => p.ProfessionalTeam).
                WithRequired(p => p.Professional).HasForeignKey(p => p.ProfessionalId);

            modelBuilder.Entity<User>().HasMany(p => p.ProfessionalTeam).
                WithRequired(p => p.TeamUser).HasForeignKey(p => p.TeamUserId);

            #endregion

            #region ProfessionalOffice

            modelBuilder.Entity<ProfessionalOffice>().
                HasMany(p => p.ProfessionalOfficeSchedules).WithRequired(p => p.ProfessionalOffice);

            modelBuilder.Entity<ProfessionalOfficeSchedule>().
                HasMany(p => p.Appointments).WithOptional(p => p.ProfessionalOfficeSchedule).HasForeignKey(p => p.ProfessionalOfficeScheduleId);

            //modelBuilder.Entity<ProfessionalOffice>().
            //modelBuilder.Entity<ContactLocation>()
            //    .HasOptional(b => b.ProfessionalOffice)
            //    .WithRequired(b => b.ContactLocation);
                //.Map(b => b.MapKey("ContactLocationId"));                        

            #endregion

            #region Customer - Contact
           
            // Extended To Main 1 on 1
            modelBuilder.Entity<Customer>()
                .HasRequired(i => i.Contact)
                .WithRequiredDependent();

            #endregion

            #region Institution            

            // Extended To Main 1 on 1
            modelBuilder.Entity<Institution>()
                .HasRequired(i => i.Contact)
                .WithRequiredDependent(); 

            #endregion

            #region InsuranceInstitution, ProfessionalInsuranceInstitutionPlan - Professional, InsuranceInstitution

            modelBuilder.Entity<InsuranceInstitutionPlan>().
               HasRequired(c => c.InsuranceInstitution).WithMany(p=>p.InsuranceInstitutionPlans);

            modelBuilder.Entity<ProfessionalInsuranceInstitutionPlan>().
               HasRequired(c => c.Professional).WithMany(p=>p.ProfessionalInsuranceInstitutionPlans);

            modelBuilder.Entity<ProfessionalInsuranceInstitutionPlan>().
               HasRequired(c => c.InsuranceInstitution).WithMany();

            modelBuilder.Entity<ProfessionalInsuranceInstitutionPlan>().
               HasRequired(c => c.InsuranceInstitutionPlan).WithMany();

            #endregion                     

            #region Appointment            

            modelBuilder.Entity<Appointment.Appointment>().
                HasRequired(p => p.Professional);

            modelBuilder.Entity<Appointment.Appointment>().HasKey(p => p.AppointmentId);
            modelBuilder.Entity<Appointment.AppointmentTransaction>().HasKey(p => p.AppointmentTransactionId);
            modelBuilder.Entity<Appointment.AppointmentTransaction>().Property(p => p.AppointmentId).IsRequired();

            modelBuilder.Entity<Appointment.Appointment>().HasMany(p => p.AppointmentTransactions).
                WithRequired(p => p.Appointment).HasForeignKey(p => p.AppointmentId); 

            //modelBuilder.Entity<Appointment.AppointmentTransaction>().
            //    HasRequired(p => p.Appointment).WithMany(p => p.AppointmentTransactions)
            //    .HasForeignKey(p => p.AppointmentId);


            modelBuilder.Entity<Customer>().HasMany(p => p.AppointmentTransactions).
                WithOptional(p => p.Customer).HasForeignKey(p => p.CustomerId);

            modelBuilder.Entity<Appointment.AppointmentTransaction>().
                HasOptional(p => p.InsuranceInstitution).WithMany(p=>p.AppointmentTransactions).HasForeignKey(p=>p.InsuranceInstitutionId);

            modelBuilder.Entity<Appointment.AppointmentTransaction>().
                HasOptional(p => p.InsuranceInstitutionPlan).WithMany();

            modelBuilder.Entity<Appointment.AppointmentTransaction>().
                HasOptional(p => p.SpecializationAppointmentReason).WithMany();


            #endregion

            #region SpecializationAppointmentReason

            modelBuilder.Entity<SpecializationAppointmentReason>().
                HasRequired(p => p.Specialization).WithRequiredDependent();

            #endregion

            #region ContactLocation
            modelBuilder.Entity<ContactLocation>().HasMany(p => p.ContactLocationPictures).WithRequired(p => p.ContactLocation);

            modelBuilder.Entity<ContactLocation>().
                HasRequired(c => c.Country).WithRequiredDependent();

            modelBuilder.Entity<ContactLocation>().
                HasRequired(c => c.State).WithRequiredDependent();

            modelBuilder.Entity<ContactLocation>().
                HasRequired(c => c.City).WithMany(p => p.ContactLocations);

            modelBuilder.Entity<ContactLocation>().
                HasRequired(c => c.Area).WithOptional();           

            #endregion

            #region Patient

            modelBuilder.Entity<Patient>().HasRequired(p => p.Contact).WithRequiredDependent();

            modelBuilder.Entity<Professional>().HasMany(p => p.ProfessionalPatients).WithRequired(p => p.Professional);

            modelBuilder.Entity<Patient>().HasMany(p => p.ProfessionalPatients).WithRequired(p => p.Patient);

            modelBuilder.Entity<Patient>().HasMany(p => p.PatientInsuranceInstitutions).WithRequired(p => p.Patient);

            modelBuilder.Entity<Patient>().HasMany(p => p.MedicalCares).WithRequired(p => p.Patient);

            modelBuilder.Entity<Patient>().HasMany(p => p.PatientVitalSigns).WithRequired(p => p.Patient);

            modelBuilder.Entity<Professional>().HasMany(p => p.PatientVitalSigns).WithRequired(p => p.Professional);

            modelBuilder.Entity<Professional>().HasMany(p => p.MedicalHistories).WithRequired(p => p.Professional);

            modelBuilder.Entity<VitalSign>().HasMany(p => p.PatientVitalSigns).WithRequired(p => p.VitalSign);

            modelBuilder.Entity<InsuranceInstitution>().HasMany(p => p.PatientInsuranceInstitutions).
                WithRequired(p => p.InsuranceInstitution).HasForeignKey(p => p.InsuranceInstitutionId);

            modelBuilder.Entity<InsuranceInstitutionPlan>().HasMany(p => p.PatientInsuranceInstitutions).
                WithOptional(p => p.InsuranceInstitutionPlan).HasForeignKey(p => p.InsuranceInstitutionPlanId);

            #endregion

            #region Medical

            modelBuilder.Entity<Vademecum>().HasMany(p => p.ProfessionalVademecums).WithOptional(p => p.Vademecum);

            modelBuilder.Entity<MedicalCare>().HasMany(p => p.MedicalCareMedications).WithRequired(p => p.MedicalCare);

            modelBuilder.Entity<ProfessionalVademecum>().HasMany(p => p.MedicalCareMedications).WithOptional(p => p.ProfessionalVademecum);

            #endregion
        }
    }
}