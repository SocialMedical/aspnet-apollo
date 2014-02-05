using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo
{
    public class Constants
    { 
        public const string EmptyContactPicture = "/Content/images/contacts/DefaultProfessional.jpg";
    }

    public enum ProfessionalOption
    {
        ProfessionalBook,
        NewCustomer,
        EditProfile,
        Offices,
        Patients,
        Epicrisis,
        Team
    }
}
