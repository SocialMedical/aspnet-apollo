using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionMailer.Net.Mvc;
using Sic;

namespace Rp3.Web.Mvc.Controllers
{
    public class MailController: ActionMailer.Net.Mvc.MailerBase
    {
        public MailController()
        {
            
        }

        public virtual string FromAccount
        {
            get
            {
                return Sic.Configuration.SicConfigurationSection.Current.EmailAgent.Name;
            }
        }

        public EmailResult SendAsEmail(string subject, List<MailContact> to, string viewName, object model = null)
        {
            this.Subject = subject;
            this.From = FromAccount;

            foreach (var t in to.Where(p => p.MailtoType == MailtoType.To))
                To.Add(t.Mail);

            foreach (var t in to.Where(p => p.MailtoType == MailtoType.Cc))
                this.CC.Add(t.Mail);

            foreach (var t in to.Where(p => p.MailtoType == MailtoType.Cco))
                this.BCC.Add(t.Mail);

            return Email(viewName, model);
        }
    }
}
