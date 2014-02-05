using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Web;
using System.Web.Hosting;
namespace Sic
{
    public class Mail
    {        
        public static bool Send(string Subject, List<MailContact> To, string Message)
        {
            return Send(Subject, null, To, Message, null);
        }

        public static bool Send(string Subject, MailContact From, List<MailContact> To, string Message, string[] InLineImages)
        {
            try
            {
                if (!MailInformation.IsLoaded)
                    MailInformation.Load();

                MailMessage message = new MailMessage();

                if (From != null)
                    message.From = new MailAddress(From.Mail, From.Name);
                else
                    message.From = new MailAddress(MailInformation.NetworkCredentialUser, MailInformation.MailContactFrom);

                foreach (MailContact contact in To)
                    message.To.Add(new MailAddress(contact.Mail, contact.Name));

                if(InLineImages != null)
                {
                    foreach (string filePath in InLineImages)
                    {
                        System.Net.Mail.Attachment inlineAtt = new System.Net.Mail.Attachment(HostingEnvironment.MapPath(filePath));
                        inlineAtt.ContentDisposition.Inline = true;
                        message.Attachments.Add(inlineAtt);
                    }
                }

                message.Subject = Subject;
                message.BodyEncoding = System.Text.Encoding.Default;
                message.Body = Message;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient(Sic.MailInformation.NetworkSMTP);
                client.Port = Sic.MailInformation.NetworkPort;
                client.Credentials = new System.Net.NetworkCredential(Sic.MailInformation.NetworkCredentialUser, Sic.MailInformation.NetworkCredentialPWD);
                client.EnableSsl = Sic.MailInformation.EnableSsl;

                client.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class MailInformation
    {
        internal static bool IsLoaded { get; set; }

        internal static void Load()
        {
            //NetworkSMTP = "mail.sic.com.ec";
            //NetworkPort = 25;
            //NetworkCredentialUser = "jguzhnay@sic.com.ec";
            //NetworkCredentialPWD = "12345678";
            //EnableSsl = false;

            NetworkSMTP = "smtp.gmail.com";
            NetworkPort = 25;
            NetworkCredentialUser = "socialmedicalec@gmail.com";
            NetworkCredentialPWD = "quehubo123";
            EnableSsl = false;            

            MailContactFrom = "Social Medical";
        }

        public static string NetworkSMTP { get; set; }
        public static int NetworkPort { get; set; }
        public static string NetworkCredentialUser { get; set; }
        public static string NetworkCredentialPWD { get; set; }
        public static bool EnableSsl { get; set; }
        public static string MailContactFrom { get; set; }
    }

    public class MailContact
    {
        public string Name { get; set; }
        public string Mail { get; set; }

        public MailContact() { }

        public MailContact(string Name, string Mail)
        {
            this.Name = Name;
            this.Mail = Mail;
        }
    }
}
