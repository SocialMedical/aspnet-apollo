using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models.Pro.View;
using Sic.Apollo.Models.Appointment.View;
using Sic.Data.Entity;
using System.Linq.Expressions;
using Sic.Apollo.Models.Medical;

namespace Sic.Apollo.Models.Repositories
{
    public class ProfessionalRepository : Sic.Data.Entity.Repository<Sic.Apollo.Models.Pro.Professional>
    {
        public ProfessionalRepository(DbContext context)
            :base(context)
        {
        }

        public override Sic.Apollo.Models.Pro.Professional GetByID(object id)
        {
            int key = Convert.ToInt32(id);
            return Get(p => p.ProfessionalId == key, "Contact").SingleOrDefault();
        }

        public ProfessionalScore GetScore(int professionalId)
        {
            Context db = (Context)context;

            return (from p in db.AppointmentTransactions
                        join a in db.Appointments on p.AppointmentId equals a.AppointmentId
                        where p.State == (int)AppointmentState.Rated
                        group new { p, a } by a.ProfessionalId into g
                        select new ProfessionalScore
                        {
                            SumRateScore1 = g.Sum(p => p.p.RateScore1) ?? 0,
                            SumRateScore2 = g.Sum(p => p.p.RateScore2) ?? 0,
                            SumRateScore3 = g.Sum(p => p.p.RateScore3) ?? 0,
                            CountRateScore = g.Count()
                        }).FirstOrDefault(); 
        }

        public List<Pro.View.Professional> GetProfessionalsPendingConfirmation()
        {
            Context db = (Context)context;

            return (from p in db.Professionals                                                  
                         join u in db.Users on p.ProfessionalId equals u.UserId
                         where u.State == (int)UserState.PendingConfirmation
                         select new Pro.View.Professional
                         {
                             ProfessionalId = p.ProfessionalId,                             
                             Picture = p.Contact.Picture,                             
                             FirstName = p.Contact.FirstName,
                             MiddleName = p.Contact.MiddleName,
                             LastName = p.Contact.LastName,
                             SecondLastName = p.Contact.SecondLastName,                             
                             RateScore = p.RateScore,
                             RegisterDate = u.RegisterDate,
                             UserState = u.State
                         }).ToList();
        }

        public List<Pro.View.Professional> GetProfessionals(int? professionalId = null)
        {
            Context db = (Context)context;

            var query = (from p in db.Professionals
                         join u in db.Users on p.ProfessionalId equals u.UserId
                         select new Pro.View.Professional
                         {
                             ProfessionalId = p.ProfessionalId,                            
                             Picture = p.Contact.Picture,                             
                             FirstName = p.Contact.FirstName,
                             MiddleName = p.Contact.MiddleName,
                             LastName = p.Contact.LastName,
                             SecondLastName = p.Contact.SecondLastName,                            
                             RateScore = p.RateScore,
                             UserState = u.State
                         });

            if (professionalId != null)
                query = query.Where(p => p.ProfessionalId == professionalId);

            return query.ToList();
        }

        public List<Pro.View.Professional> GetProfessionals(int? specialityId = null,
            int? cityId = null, int? insuranceInstitutionId = null,
            int? insuranceInstitutionPlanId = null, string professionalName = null, 
            int? contactLocationId = null, int? professionalId = null,
            List<int> userState = null)
        {
            Context db = (Context)context;

            #region Professional Query

            var query = (from p in db.Professionals
                         join u in db.Users on p.ProfessionalId equals u.UserId

                         join s in db.ProfessionalSpecializations on p.ProfessionalId equals s.ProfessionalId                        
                         join l in db.ProfessionalOffices on p.ProfessionalId equals l.ProfessionalId

                         select new Pro.View.Professional
                         {
                             ProfessionalId = p.ProfessionalId,
                             CityId = l.CityId,
                             Picture = p.Contact.Picture,
                             SpecializationId = s.SpecializationId,
                             SpecializationName = s.Specialization.Name,                             
                             ProfessionalDescription = p.ProfessionalDescription,
                             FirstName = p.Contact.FirstName,
                             MiddleName = p.Contact.MiddleName,
                             LastName = p.Contact.LastName,
                             SecondLastName = p.Contact.SecondLastName,
                             ContactLocationId = l.ContactLocationId,
                             Address = l.Address,                             
                             DefaultPhoneNumber = l.DefaultPhoneNumber,
                             DefaultPhoneExtension = l.DefaultPhoneExtension,
                             Latitude = l.Latitude,
                             Longitude = l.Longitude,
                             RateScore = p.RateScore,
                             RateScore1 = p.RateScore1,
                             RateScore2 = p.RateScore2,
                             RateScore3 = p.RateScore3,
                             UserState = u.State
                         } );

            if (specialityId != null)
                query = query.Where(p => p.SpecializationId == specialityId.Value);

            if (cityId != null)
                query = query.Where(p => p.CityId == cityId.Value);

            if(insuranceInstitutionId != null)
            {
                query = from q in query
                        join ins in db.ProfessionalInsuranceInstitutionPlans
                        on q.ProfessionalId equals ins.ProfessionalId
                        where ins.InstitutionId == insuranceInstitutionId
                        select q;
            }

            if (contactLocationId != null)
                query = query.Where(p => p.ContactLocationId == contactLocationId);

            if (professionalId != null)
                query = query.Where(p => p.ProfessionalId == professionalId);

            if(userState != null)
                query = query.Where(p => userState.Contains(p.UserState));

            if (!string.IsNullOrEmpty(professionalName))
            {
                string[] valueNames = null;
                if (professionalName.Any(p => p == ','))
                    valueNames = professionalName.Split(',');
                else
                    valueNames = professionalName.Split(' ');

                foreach (string name in valueNames)
                {
                    Expression<Func<Pro.View.Professional, bool>> filter = p=>(p.FirstName + " " + p.LastName).Contains(name);

                    query = query.Where(filter);
                }               
            }

            var professionals = query.ToList();

            int markerIndex = 1;

            foreach (var p in professionals)
            {
                p.MarkerIndex = markerIndex;                
                
                markerIndex++;               

                if (markerIndex >= 100)
                    markerIndex = 100;
            }

            #endregion           

            return professionals;
        }

        #region Vademecum

        public IEnumerable<Medical.View.Vademecum> FindVademecums(string textSearch, int professionalId)
        {
            Context db = (Context)context;
            string query = QueryFindVademecum.Replace("@SearchText",textSearch).Replace("@ProfessionalId", professionalId.ToString());

            IEnumerable<Medical.View.Vademecum> result = db.Database.SqlQuery<Medical.View.Vademecum>(query.ToString());
            return result;            
        }

        #endregion

        public IEnumerable<InsuranceInstitution> GetInsuranceInstitutions(int professionalId)
        {
            Context db = (Context)context;

            return (from a in db.ProfessionalInsuranceInstitutionPlans
                    join b in db.InsuranceInstitutions.Include("Contact") on a.InstitutionId equals b.InstitutionId
                    where a.ProfessionalId == professionalId
                    select b).Distinct().ToList();
        }

        public Models.Pro.View.ProfessionalSummary GetPofessionalSummary(int professionalId)
        {
            Context db = (Context)context;
            int pendigConfirmation = (int)AppointmentState.PendingConfirmation;
            int confirmed = (int)AppointmentState.Confirmed;
            //int rated = (int)AppointmentState.Rated;

            DateTime currentDateTime = DateTime.Now.Date;

            var query = from p in db.Professionals
                        where p.ProfessionalId == professionalId
                        select new Models.Pro.View.ProfessionalSummary
                        {
                            ProfessionalId = p.ProfessionalId,
                            FirstName = p.Contact.FirstName,
                            MiddleName = p.Contact.MiddleName,
                            LastName = p.Contact.LastName,
                            SecondLastName = p.Contact.SecondLastName,
                            Picture = p.Contact.Picture,
                            RateScore = p.RateScore,
                            //CommentsCount = p.s grp.Sum(p => (p.State == rated ? 1 : 0)),
                            //CustomerCount = 0,
                            PatientCount = p.ProfessionalPatients.Count,
                            OfficeCount = p.ProfessionalOffices.Count,
                            //AppointmentCount = grp.Count(),
                            AppointmentPendingConfirmationToAttentionCount = p.Appointments.Where(q => q.State == pendigConfirmation && q.StartDate > currentDateTime).Count(),
                            AppointmentPendingCount = p.Appointments.Where(q => q.State == confirmed && q.StartDate >= currentDateTime).Count(),
                            AppointmentPendingCheckAttentionCount = p.Appointments.Where(q => q.State == confirmed && q.StartDate < currentDateTime).Count()
                        };
                                        
            var pro = query.FirstOrDefault();
            pro.PictureMin = Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(pro.Picture);
            return pro;
        }

        public int GetPatientCount(int ProfessionalId)
        {
            Context db = (Context)context;
            byte active = (byte)PatientState.Active;
            return db.ProfessionalPatients.Where(p => p.ProfessionalId == ProfessionalId && p.State == active).Count();
        }

        public int GetAppointmentPendingCount(int professionalId)
        {
            Context db = (Context)context;
            int state = (int)AppointmentState.Confirmed;
            DateTime currentDateTime = DateTime.Now.Date;
            return db.Appointments.Where(p =>p.ProfessionalId == professionalId && p.State == state && p.StartDate >= currentDateTime).Count();
        }

        #region        
        const string QueryFindVademecum = "SELECT TOP 15 ProfessionalVademecumId,VademecumId,Name = '* ' + Name,Posology " +
                                          "FROM med.tbProfessionalVademecum " +
                                          "WHERE Active = 1 " +
                                          "AND ProfessionalId = @ProfessionalId " +
                                          "AND Name LIKE '%@SearchText%' " +
                                          "UNION ALL " +
                                          "SELECT TOP 15 ProfessionalVademecumId = 0,VademecumId,Name,Posology = '' " +
                                          "FROM med.tbVademecum " +
                                          "WHERE Active = 1 " +
                                          "AND Name LIKE '%@SearchText%'";

        #endregion
    }
}