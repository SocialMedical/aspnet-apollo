using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sic.Apollo.Models.Medical;
using Sic.Data.Entity;
using Sic.Apollo.Models.General;
using Sic.Apollo.Models.General.View;

namespace Sic.Apollo.Models.Repositories
{
    public class PatientRepository: Sic.Data.Entity.Repository<Patient>
    {
        public PatientRepository(DbContext context)
            :base(context)
        {
        }        

        public IEnumerable<Person> GetPatientPerson(string textSearch, int professionalId)
        {
            string[] entries = null;
            if (textSearch.Contains(","))            
                entries = textSearch.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            else            
                entries = textSearch.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);

            StringBuilder queryString = new StringBuilder();
            queryString.Append(QueryFindPatient.Replace("@ProfessionalId", professionalId.ToString()).
                Replace("@ActiveState", ((byte)PatientState.Active).ToString()));            
            
            for (int i = 0; i <entries.Count();i++ )
            {
                if(i == entries.Count()-1)
                    queryString.AppendLine(QueryNameContaint.Replace("@SearchText", entries[i]));
                else
                    queryString.AppendLine(QueryNameEqual.Replace("@SearchText", entries[i]));
            }

            Context db = context as Context;
            IEnumerable<Person> result = db.Database.SqlQuery<Person>(queryString.ToString());
            return result;
        }


        #region Query

        const string QueryFindPatient =
            "SELECT TOP 15 patientContact.ContactId,LastName,SecondLastName,FirstName,MiddleName" +
            " FROM med.tbProfessionalPatient relation" +
            " JOIN med.tbPatient patient" +
            " ON relation.PatientId = patient.PatientId" +
            " JOIN gen.tbContact patientContact" +
            " ON patient.PatientId = patientContact.ContactId" +
            " WHERE relation.ProfessionalId = @ProfessionalId AND relation.State = @ActiveState";

        const string QueryNameContaint = " AND ( FirstName LIKE '@SearchText%'" +
	        "OR MiddleName LIKE '@SearchText%'" +
	        "OR LastName LIKE '@SearchText%'" +
	        "OR SecondLastName LIKE '@SearchText%')";

        const string QueryNameEqual = " AND ( FirstName = '@SearchText'" +
            "OR MiddleName = '@SearchText'" +
            "OR LastName = '@SearchText'" +
            "OR SecondLastName = '@SearchText')";

        #endregion
    }
}
