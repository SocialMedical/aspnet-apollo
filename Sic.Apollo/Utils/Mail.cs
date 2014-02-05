using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace Sic.Apollo
{
    public class Mail
    {
        #region Letter

        public static bool SendLetter(string Subject, List<MailContact> To, string Header, string Body)
        {
            string applicationUrl = @"50.61.158.7";

            string template = Utils.FileToString(@"~/Content/templates/mail/letter.html");
                
            template = template.Replace("[@Header]", Header);
            template = template.Replace("[@Body]", Body);

            template = template.Replace("[@UrlTopImage]", String.Format("http://{0}{1}", applicationUrl, "/Content/images/mail/letter/top.png"));
            template = template.Replace("[@UrlBottomImage]", String.Format("http://{0}{1}", applicationUrl, "/Content/images/mail/letter/btm.png"));

            return Sic.Mail.Send(Subject, To, template);
        }

        #endregion Letter

        #region Welcome

        public static bool SendWelcome(UserType userType, string userFullName, string userMail, Gender gender)
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty;

            switch(userType)
            {
                case UserType.Customer:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForWelcomeCustomerSubject, Sic.Apollo.Resources.Resources.LabelForApplicationName);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForCustomerHeader, userFullName,
                      (gender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                      );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForWelcomeCustomerBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl);

                    break;

                case UserType.Professional:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForWelcomeProfessionalSubject, Sic.Apollo.Resources.Resources.LabelForApplicationName);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, userFullName,
                        (gender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (gender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForWelcomeProfessionalBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl);

                    break;

                default: return false;
            }

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(userFullName, userMail) }, header, body);
        }

        #endregion Welcome

        #region Professional Confirmed

        public static bool SendProfessionalConfirmed(string userFullName, string userMail, Gender gender)
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty;

            subject = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalConfirmedSubject, Sic.Apollo.Resources.Resources.LabelForApplicationName);

            header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, userFullName,
                        (gender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (gender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

            body = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalConfirmedBody,
                Sic.Apollo.Resources.Resources.LabelForApplicationName,
                Sic.Apollo.Resources.Resources.LabelForApplicationUrl);

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(userFullName, userMail) }, header, body);
        }

        #endregion Professional Confirmed

        #region Appintment Booked

        public static bool SendAppointmentBooked(UserType userType,
            string professionalFullName, string professionalMail, Gender professionalGender, string professionalOfficeAdress, string professionalPhoneNumber,
            string customerFullName, string customerMail, Gender customerGender, string customerPhoneNumber,
            DateTime appointmentStartDate, DateTime appointmentEndDate
            )
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty, contactFullName = String.Empty, contactMail = String.Empty;
            int duration = Convert.ToInt32((appointmentEndDate - appointmentStartDate).TotalMinutes);

            switch (userType)
            {
                case UserType.Customer:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentBookedCustomerSubject, professionalFullName, appointmentStartDate,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForCustomerHeader, customerFullName,
                        (customerGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentBookedCustomerBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        professionalFullName, appointmentStartDate, duration, professionalOfficeAdress, professionalPhoneNumber,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    contactFullName = customerFullName;
                    contactMail = customerMail;

                    break;

                case UserType.Professional:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentBookedProfessionalSubject, customerFullName, appointmentStartDate);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, professionalFullName,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentBookedProfessionalBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        customerFullName, appointmentStartDate, duration, professionalOfficeAdress, customerPhoneNumber
                        );

                    contactFullName = professionalFullName;
                    contactMail = professionalMail;

                    break;

                default: return false;
            }

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(contactFullName, contactMail) }, header, body);
        }

        #endregion Appintment Booked

        #region Appointment Confirmed

        public static bool SendAppointmentConfirmed(UserType userType,
            string professionalFullName, string professionalMail, Gender professionalGender, string professionalOfficeAdress, string professionalPhoneNumber,
            string customerFullName, string customerMail, Gender customerGender, string customerPhoneNumber,
            DateTime appointmentStartDate, DateTime appointmentEndDate
            )
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty, contactFullName = String.Empty, contactMail = String.Empty;
            int duration = Convert.ToInt32((appointmentEndDate - appointmentStartDate).TotalMinutes);

            switch (userType)
            {
                case UserType.Customer:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentConfirmedCustomerSubject, professionalFullName, appointmentStartDate,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForCustomerHeader, customerFullName,
                        (customerGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentConfirmedCustomerBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        professionalFullName, appointmentStartDate, duration, professionalOfficeAdress, professionalPhoneNumber,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    contactFullName = customerFullName;
                    contactMail = customerMail;

                    break;

                case UserType.Professional:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentConfirmedProfessionalSubject, customerFullName, appointmentStartDate);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, professionalFullName,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentConfirmedProfessionalBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        customerFullName, appointmentStartDate, duration, professionalOfficeAdress, customerPhoneNumber
                        );

                    contactFullName = professionalFullName;
                    contactMail = professionalMail;

                    break;

                default: return false;
            }

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(contactFullName, contactMail) }, header, body);
        }

        #endregion Appointment Confirmed

        #region Appointment Canceled

        public static bool SendAppointmentCanceled(UserType userType,
            string professionalFullName, string professionalMail, Gender professionalGender, string professionalOfficeAdress, string professionalPhoneNumber,
            string customerFullName, string customerMail, Gender customerGender, string customerPhoneNumber,
            DateTime appointmentStartDate, DateTime appointmentEndDate
            )
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty, contactFullName = String.Empty, contactMail = String.Empty;
            int duration = Convert.ToInt32((appointmentEndDate - appointmentStartDate).TotalMinutes);

            switch (userType)
            {
                case UserType.Customer:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledCustomerSubject, professionalFullName, appointmentStartDate,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForCustomerHeader, customerFullName,
                        (customerGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledCustomerBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        professionalFullName, appointmentStartDate, duration, professionalOfficeAdress, professionalPhoneNumber,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    contactFullName = customerFullName;
                    contactMail = customerMail;

                    break;

                case UserType.Professional:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledProfessionalSubject, customerFullName, appointmentStartDate);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, professionalFullName,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledProfessionalBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        customerFullName, appointmentStartDate, duration, professionalOfficeAdress, customerPhoneNumber
                        );

                    contactFullName = professionalFullName;
                    contactMail = professionalMail;

                    break;

                default: return false;
            }

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(contactFullName, contactMail) }, header, body);
        }

        #endregion Appointment Canceled

        #region Appointment Rescheduled

        public static bool SendAppointmentRescheduled(UserType userType,
            string professionalFullName, string professionalMail, Gender professionalGender, string professionalOfficeAdress, string professionalPhoneNumber,
            string customerFullName, string customerMail, Gender customerGender, string customerPhoneNumber,
            DateTime appointmentStartDate, DateTime appointmentEndDate
            )
        {
            string subject = String.Empty, header = String.Empty, body = String.Empty, contactFullName = String.Empty, contactMail = String.Empty;
            int duration = Convert.ToInt32((appointmentEndDate - appointmentStartDate).TotalMinutes);

            switch (userType)
            {
                case UserType.Customer:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledCustomerSubject, professionalFullName, appointmentStartDate,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForCustomerHeader, customerFullName,
                        (customerGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledCustomerBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        professionalFullName, appointmentStartDate, duration, professionalOfficeAdress, professionalPhoneNumber,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForHe : Sic.Apollo.Resources.Resources.LabelForShe)
                        );

                    contactFullName = customerFullName;
                    contactMail = customerMail;

                    break;

                case UserType.Professional:

                    subject = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledProfessionalSubject, customerFullName, appointmentStartDate);

                    header = String.Format(Sic.Apollo.Resources.Resources.MailForProfessionalHeader, professionalFullName,
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForProfessionalMaleShort : Sic.Apollo.Resources.Resources.LabelForProfessionalFemaleShort),
                        (professionalGender == Gender.Male ? Sic.Apollo.Resources.Resources.LabelForDearMale : Sic.Apollo.Resources.Resources.LabelForDearFemale)
                        );

                    body = String.Format(Sic.Apollo.Resources.Resources.MailForAppointmentCanceledProfessionalBody,
                        Sic.Apollo.Resources.Resources.LabelForApplicationName,
                        Sic.Apollo.Resources.Resources.LabelForApplicationUrl,
                        customerFullName, appointmentStartDate, duration, professionalOfficeAdress, customerPhoneNumber
                        );

                    contactFullName = professionalFullName;
                    contactMail = professionalMail;

                    break;

                default: return false;
            }

            return Mail.SendLetter(subject, new List<MailContact>() { new MailContact(contactFullName, contactMail) }, header, body);
        }

        #endregion Appointment Rescheduled
    }
}