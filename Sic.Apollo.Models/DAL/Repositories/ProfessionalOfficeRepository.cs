using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Data.Entity;
using Sic.Apollo.Models.Pro;
using Sic.Data;

namespace Sic.Apollo.Models.Repositories
{
    public class ProfessionalOfficeRepository: Repository<ProfessionalOffice>
    {
        public ProfessionalOfficeRepository(DbContext context)
            :base(context)
        {
        }

        #region QueryString

        string QueryInsert = "INSERT INTO gen.tbContactLocation (" +
                             "ContactId," +
                             "Description," +
                             "Priority," +
                             "LocationType," +
                             "CountryId," +
                             "StateId," +
                             "CityId," +
                             "AreaId," +
                             "Latitude," +
                             "Longitude," +
                             "Address," +                             
                             "[References]," +
                             "PostalCode," +
                             "POBox," +
                             "DefaultPhoneNumber," +
                             "DefaultPhoneExtension," +
                             "PhoneNumber01," +
                             "PhoneExtension01," +
                             "PhoneNumber02," +
                             "PhoneExtension02," +
                             "Email) " +
                             "VALUES(" +
                             "@ContactId," +
                             "@Description," +
                             "@Priority," +
                             "@LocationType," +
                             "@CountryId," +
                             "@StateId," +
                             "@CityId," +
                             "@AreaId," +
                             "@Latitude," +
                             "@Longitude," +
                             "@Address," +                             
                             "@References," +
                             "@PostalCode," +
                             "@POBox," +
                             "@DefaultPhoneNumber," +
                             "@DefaultPhoneExtension," +
                             "@PhoneNumber01," +
                             "@PhoneExtension01," +
                             "@PhoneNumber02," +
                             "@PhoneExtension02," +
                             "@Email) " +
                             "DECLARE @ContactLocationId INT " +
                             "SET @ContactLocationId = @@IDENTITY " +
                             "INSERT INTO pro.tbProfessionalOffice (" +
                             "ContactLocationId," +
	                         "ProfessionalId," +
	                         "Active) VALUES(" +
                             "@ContactLocationId," +
                             "@ProfessionalId," +
                             "@Active)";

        #endregion        
        
        public override void Insert(ProfessionalOffice entity)
        {
            context.PendingCommands.Add(Parameter.ApplyParameters(QueryInsert, 
                entity.GetParameters(new string[] { "ContactLocationId" }))); 
        }
    }
}