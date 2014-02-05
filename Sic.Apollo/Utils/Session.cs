using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sic.Apollo
{
    public static class Session
    {       
        public static int ProfessionalId
        {
            get
            {
                return Convert.ToInt32(Sic.Web.Mvc.Session.GetValue("ProfessionalId"));
            }
            set
            {
                Sic.Web.Mvc.Session.SetValue("ProfessionalId", value);
            }
        }
    }
}