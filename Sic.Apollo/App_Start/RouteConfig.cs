using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sic.Apollo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Account

            routes.MapRoute(
              "Register Customer",
              "Registro/Paciente",
              new
              {
                  controller = "Account",
                  action = "RegisterCustomer"
              }
           );

            routes.MapRoute(
              "Register Doctor",
              "Registro/Doctor",
              new
              {
                  controller = "Account",
                  action = "RegisterProfessional"
              }
           );

            routes.MapRoute(
              "Register",
              "Registro",
              new
              {
                  controller = "Account",
                  action = "SelectUserType"
              }
           );

            #endregion

            #region Professional Search

            // routes.MapRoute(
            //    "Search Insurance Default",
            //    "Search/{specializationId}/{cityId}/{insuranceInstitutionId}",
            //    new
            //    {
            //        controller = "Professional",
            //        action = "Search",
            //        insuranceInstitutionId =  UrlParameter.Optional
            //    }
            //);

            // routes.MapRoute(
            //    "Search All",
            //    "Search/{specializationId}/{cityId}/{insuranceInstitutionId}/{professionalName}",
            //    new
            //    {
            //        controller = "Professional",
            //        action = "Search",
            //        specializationId = UrlParameter.Optional,
            //        cityId  = UrlParameter.Optional,
            //        insuranceInstitutionId = UrlParameter.Optional,
            //        professionalName = UrlParameter.Optional
            //    }
            //);

            routes.MapRoute(
               "Search All",
               "Busqueda/Medicos/{*pathInfo}",
               new
               {
                   controller = "Professional",
                   action = "Search"
               }
           );

            #endregion

            #region Professional Presentation

            routes.MapRoute(
               "Professional Presentation",
               "Doctor/{professional}",
               new
               {
                   controller = "Professional",
                   action = "Presentation"
               }
           );

            #endregion

            #region Professional Profile

            routes.MapRoute(
               "Professional Profile",
               "Doctor/Profile/Main",
               new
               {
                   controller = "Professional",
                   action = "Profile"
               }
            );

            routes.MapRoute(
               "Professional Office",
               "Doctor/Profile/Office",
               new
               {
                   controller = "Professional",
                   action = "Offices"
               }
            );

            routes.MapRoute(
               "Professional Schools",
               "Doctor/Profile/Schools",
               new
               {
                   controller = "Professional",
                   action = "Schools"
               }
            );

            routes.MapRoute(
               "Professional Experiences",
               "Doctor/Profile/Experiences",
               new
               {
                   controller = "Professional",
                   action = "Experiences"
               }
            );

            routes.MapRoute(
               "Professional Communities",
               "Doctor/Profile/Communities",
               new
               {
                   controller = "Professional",
                   action = "Communities"
               }
            );

            routes.MapRoute(
               "Professional Insurances",
               "Doctor/Profile/Insurances",
               new
               {
                   controller = "Professional",
                   action = "InsuranceInstitutionPlans"
               }
            );

            routes.MapRoute(
               "Professional Insurances Edit",
               "Doctor/Profile/Insurances/Edit",
               new
               {
                   controller = "Professional",
                   action = "EditInsuranceInstitutionPlans"
               }
            );

            routes.MapRoute(
               "Professional Scheduling Offices",
               "Doctor/Profile/Offices/Scheduling",
               new
               {
                   controller = "Professional",
                   action = "OfficesScheduling"
               }
            );

            routes.MapRoute(
               "Professional Scheduling Edit",
               "Doctor/Profile/Offices/Scheduling/Edit",
               new
               {
                   controller = "Professional",
                   action = "EditOfficeSchedule"
               }
            );

            #endregion

            #region Professional Appointment

            routes.MapRoute(
               "Professional Appointment",
               "Doctor/Appointments/Pending",
               new
               {
                   controller = "Appointment",
                   action = "PendingConfirmToAttention"
               }
            );

            routes.MapRoute(
               "Professional Appointment Calendar",
               "Doctor/Appointment/Book",
               new
               {
                   controller = "Appointment",
                   action = "ProfessionalBook"
               }
            );

            routes.MapRoute(
               "Professional Appointment Historial",
               "Doctor/Appointments/Historial",
               new
               {
                   controller = "Appointment",
                   action = "ProfessionalAppointmentsHistorial"
               }
            );

            routes.MapRoute(
               "Professional Appointment Confirmed",
               "Doctor/Appointments/AttendancePending/Confirm",
               new
               {
                   controller = "Appointment",
                   action = "ConfirmAttendancePending"
               }
            );

            routes.MapRoute(
               "Customer Appointment",
               "Patient/Appointments",
               new
               {
                   controller = "Appointment",
                   action = "CustomerMyAppointments"
               }
            );

            routes.MapRoute(
               "Customer Appointment PendingRatings",
               "Patient/Appointments/PendingRatings",
               new
               {
                   controller = "Appointment",
                   action = "CustomerPendingRatings"
               }
            );



            #endregion

            #region Appointment

            routes.MapRoute(
               "Appointment Process Details",
               "Appointment/Details/{appointmentId}/{professionalId}/{customerId}/{specializationId}/{specializationAppointmentReasonId}/{applyInsurance}-{applyInsuranceInstitutionId}",
               new
               {
                   controller = "Appointment",
                   action = "Details"
               }
           );

            routes.MapRoute(
               "Appointment Process Finish",
               "Appointment/Finished/{appointmentTransactionId}",
               new
               {
                   controller = "Appointment",
                   action = "Finished"
               }
           );

            //routes.MapRoute(
            //    "Appointment Process Finish",
            //    "Appointment/Details/{appointmentId}-{customerId}-{specializationId}-{specializationAppointmentReasonId}",
            //    new
            //    {
            //        controller = "Appointment",
            //        action = "Details"
            //    }
            //);

            #endregion

            routes.MapRoute(
              "Search by Description",
              "Medicos/{professionInPlural}/{cityOrInsuranceInstitution}",
              new
              {
                  controller = "Professional",
                  action = "SearchByDescription",
                  cityOrInsuranceInstitution = UrlParameter.Optional
              }
           );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


            //           routes.MapRoute(
            //   "Product",
            //   "Product/{productId}",
            //   new {controller="Product", action="Details"},
            //   new {productId = @"\d+" }
            //); Solo Integer Parametro
        }
    }
}