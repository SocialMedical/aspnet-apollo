using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models;
using Sic.Apollo.Models.General;
using System.Text;
using Sic.Apollo.Models.Pro.View;
using Sic.Web.Mvc;
using Sic.Apollo.Models.Security;
using Sic.Apollo.Models.Security.View;
using Sic.Apollo.Areas.Public.Controllers;

namespace Sic.Apollo.Areas.Public.Controllers
{
    public class ProfessionalController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {        
        public static string DefaultAction = "";

        #region Search                                      

        public ActionResult Search(int? specializationId = null, int? cityId = null,
            int? insuranceInstitutionId = null, string professionalName = null)
        {
            ViewBag.IsDescriptionSearch = false;

            if (specializationId == null && string.IsNullOrEmpty(professionalName))
            {
                specializationId = DataBase.Specializations.Get(p => p.IsDefault).SingleOrDefault().SpecializationId;
                return RedirectToAction("Search", new  { specializationId= specializationId, cityId= cityId, insuranceInstitutionId = insuranceInstitutionId, professionalName = professionalName });
            }

            HomeController.FillSearchCriteria(this, DataBase, specializationId, cityId, insuranceInstitutionId, professionalName, true);

            var result = PrepareSearchResult(specializationId, cityId, insuranceInstitutionId, professionalName);

            return View(result);
        }

        public ActionResult SearchByDescription(string professionInPlural, string cityOrInsuranceInstitution = null)
        {
            ViewBag.IsDescriptionSearch = true;
            professionInPlural = professionInPlural.ToOriginalUrlStringParameter();
            var specialization = DataBase.Specializations.Get(p => p.ProfessionInPlural == professionInPlural).SingleOrDefault();

            int? insuranceInstituitionId = null;
            int? cityId = null;
            if (!string.IsNullOrEmpty(cityOrInsuranceInstitution))
            {
                cityOrInsuranceInstitution = cityOrInsuranceInstitution.ToOriginalUrlStringParameter();
                var insurance = DataBase.InsuranceInstitutions.Get(p =>
                    p.Contact.FirstName == cityOrInsuranceInstitution).SingleOrDefault();

                if (insurance != null)
                    insuranceInstituitionId = insurance.InstitutionId;
                else
                {                    
                    var cityEntity = DataBase.Cities.Get(p => p.Name == cityOrInsuranceInstitution).SingleOrDefault();
                    if(cityEntity!=null)
                        cityId = cityEntity.CityId;
                }
            }

            HomeController.FillSearchCriteria(this, DataBase, specialization.SpecializationId, cityId, insuranceInstituitionId, null, true);            

            var result = PrepareSearchResult(specialization.SpecializationId, null, null, null);            

            return View("Search", result);
        }

        private List<Models.Pro.View.Professional> PrepareSearchResult(int? specializationId = null, int? cityId = null,
            int? insuranceInstitutionId = null, string professionalName = null)
        {            
            var result = DataBase.Professionals.GetProfessionals(
                specializationId,
                cityId,
                insuranceInstitutionId,
                insuranceInstitutionId,
                professionalName, null, null,
                new List<int>() { (int)UserState.Active }
                );

            if (result.Any())
                ViewBag.SpecializationValue = result.First().SpecializationId;
            else
                ViewBag.SpecializationValue = 0;

            int[] locations = result.Select(p => p.ContactLocationId).ToArray();
            StringBuilder filter = new StringBuilder();

            foreach (int l in locations)
            {
                if (!string.IsNullOrWhiteSpace(filter.ToString()))
                    filter.Append('|');
                filter.Append(l);
            }

            ViewBag.LocationsId = filter;

            ViewBag.TitleForSearch = string.Format(Sic.Apollo.Resources.Resources.TitleForProfessionalSearchResult, ViewBag.ProfessionName);

            return result;
        }

        #endregion

        #region Profile               
                

        public ActionResult Presentation(string professional, int? specializationId = null, int? contactLocationId = null, int? professionalId = null)
        {
            if (professionalId == null || professionalId == 0)
            {
                if (!string.IsNullOrEmpty(professional))
                {
                    if (System.Text.RegularExpressions.Regex.Match(professional, @".*\-\d*").Length > 0)
                    {
                        string[] entries = professional.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        professionalId = Convert.ToInt32(entries[entries.Count() - 1]);
                    }
                }
            }

            return ResultPresentationProfile(professionalId ?? 0, specializationId, contactLocationId, professional);
        }

        private ActionResult ResultPresentationProfile(int professionalId, int? specializationId = null, int? contactLocationId = null, string professionalUrlParameter = "")
        {
            var professionalEntity = DataBase.Professionals.Get(p=>p.ProfessionalId == professionalId, includeProperties:
                "Contact,ProfessionalCommunities,ProfessionalExperiences,ProfessionalOffices,ProfessionalSchools,ProfessionalSpecializations,ProfessionalInsuranceInstitutionPlans").Single();

            if (professionalEntity.UrlParameter != professionalUrlParameter)
            {
                return RedirectToAction("Presentation", new { professional = professionalEntity.UrlParameter, specializationId = specializationId, contactLocationId = contactLocationId });
            }

            if (specializationId == null || specializationId == 0)
            {
                var specialization = professionalEntity.ProfessionalSpecializations.FirstOrDefault();
                if (specialization != null)
                    specializationId = specialization.SpecializationId;
            }

            if (contactLocationId == null || contactLocationId == 0)
            {
                var office = professionalEntity.ProfessionalOffices.FirstOrDefault();
                if (office != null)
                    contactLocationId = office.ContactLocationId;
            }

            if (specializationId.HasValue)
                ViewBag.SpecializationId = specializationId.Value;

            if (contactLocationId.HasValue)
                ViewBag.ContactLocationId = contactLocationId.Value;

            var ListMap = new List<Sic.Apollo.Models.Pro.View.Professional>();

            int marker = 1;
            foreach (var office in professionalEntity.ProfessionalOffices)
            {
                office.MarkerIndex = office.ContactLocationId == contactLocationId ? 1 : ++marker;
                ListMap.Add(new Sic.Apollo.Models.Pro.View.Professional()
                {
                    ProfessionalId = office.ProfessionalId,
                    FirstName = office.Address,
                    MarkerIndex = office.MarkerIndex,
                    ContactLocationId = office.ContactLocationId,
                    Latitude = office.Latitude,
                    Longitude = office.Longitude
                });
            }

            ViewBag.ListMap = ListMap;

            return View("Presentation", professionalEntity);
        }

        [ChildAction]
        public ActionResult OfficePresentation(int contactLocationId, int marker, int specializationId)
        {
            var office = DataBase.ProfessionalOffices.Get(p => p.ContactLocationId == contactLocationId, includeProperties: "ContactLocationPictures").Single();
            office.MarkerIndex = marker;

            ViewBag.SpecializationId = specializationId;

            return PartialView("_OfficePresentation",office);
        }

        

        #endregion        

        #region Insurance Institutions

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult InsuranceInstitutionPlans(string message, string messageType)
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId, 
                includeProperties: "Contact,ProfessionalInsuranceInstitutionPlans,ProfessionalInsuranceInstitutionPlans.InsuranceInstitutionPlan.InsuranceInstitution.Contact,Contact").SingleOrDefault();

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            if (!String.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }

            return View(professional);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult InsuranceInstitutionIndex()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            return PartialView(DataBase.ProfessionalInsuranceInstitutionPlans.Get(p => p.ProfessionalId == professionalId, includeProperties: "InsuranceInstitutionPlan.InsuranceInstitution.Contact"));
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditInsuranceInstitutionPlans()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            
            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(AssignedInsuranceInstitutionPlan(professionalId));
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditInsuranceInstitutionPlans(string[] assignedPlans)
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;

            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId, includeProperties: "ProfessionalInsuranceInstitutionPlans").Single();
            
            //Delete
            var delete = professional.ProfessionalInsuranceInstitutionPlans.Where(p => !assignedPlans.Contains(p.InsuranceInstitutionPlanId.ToString())).ToList();
            foreach (var d in delete)
            {
                professional.ProfessionalInsuranceInstitutionPlans.Remove(d);
                d.Professional = null;
                DataBase.ProfessionalInsuranceInstitutionPlans.Delete(d);
            }

            //Insert            
            var assignedPlansHs = new HashSet<int>(assignedPlans.Select(p=>Convert.ToInt32(p)));

            var professionalInsurancePlan = new HashSet<string>
                (professional.ProfessionalInsuranceInstitutionPlans.Select(c => c.InsuranceInstitutionPlanId.ToString()));

            var insuranceInstitutionPlans = DataBase.InsuranceInstitutionPlans.Get(p=>
                assignedPlansHs.Contains(p.InsuranceInstitutionPlanId));

            foreach (var assigned in insuranceInstitutionPlans)
            {
                if (!professionalInsurancePlan.Contains(assigned.InsuranceInstitutionPlanId.ToString()))
                {
                    var insurance = new ProfessionalInsuranceInstitutionPlan()
                    {
                        InsuranceInstitutionPlanId = assigned.InsuranceInstitutionPlanId,
                        InstitutionId = assigned.InstitutionId,
                        ProfessionalId = Sic.Apollo.Session.ProfessionalId,
                        Active = true
                    };
                    professional.ProfessionalInsuranceInstitutionPlans.Add(insurance);
                    DataBase.ProfessionalInsuranceInstitutionPlans.Insert(insurance);
                }
            }
            
            DataBase.Professionals.Update(professional);
            DataBase.Save();
            
            //ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
            //ViewBag.MessageType = Sic.Constants.MessageType.Success;

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return RedirectToAction("InsuranceInstitutionPlans", new { message = Sic.Apollo.Resources.Resources.MessageForSaveOk, messageType = Sic.Constants.MessageType.Success });
        }

        private Models.Pro.View.AssignedInsuranceInstitutionPlan AssignedInsuranceInstitutionPlan(int professionalId)
        {
            Models.Pro.View.AssignedInsuranceInstitutionPlan model = new Models.Pro.View.AssignedInsuranceInstitutionPlan();
            var plans = DataBase.InsuranceInstitutionPlans.Get(p => p.Active && p.InsuranceInstitution.Active, includeProperties: "InsuranceInstitution,InsuranceInstitution.Contact");            

            model.Professional = DataBase.Professionals.GetByID(professionalId);

            var assignedPlans = new HashSet<int>(DataBase.ProfessionalInsuranceInstitutionPlans.
                Get(p => p.ProfessionalId == professionalId).
                Select(c => c.InsuranceInstitutionPlanId));

            foreach (var plan in plans)
            {
                model.AssignedInsuranceInstitutionPlans.Add(new AssignedInsuranceInstitutionPlanItem()
                {
                    InsuranceInstitutionPlan = plan,
                    Assigned = assignedPlans.Contains(plan.InsuranceInstitutionPlanId)
                });
            }
            
            return model;
        }

        #endregion

        #region Office

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Offices()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId,
                includeProperties: "Contact,ProfessionalOffices,ProfessionalOffices.ContactLocationPictures,ProfessionalOffices.City").SingleOrDefault();
            SetMarkers(professional.ProfessionalOffices);
            ViewBag.ProfessionalId = professionalId;
            ViewBag.ProfessionalOption = ProfessionalOption.Offices;
            return View(professional);
        }

        private void SetMarkers(IEnumerable<Models.Pro.ProfessionalOffice> offices)
        {
            int markerIndex = 1;
            foreach (var item in offices.Where(p=>p.Active).OrderBy(p=>p.ContactLocationId))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult OfficeIndex()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            ViewBag.ProfessionalId = professionalId;

            var offices = DataBase.ProfessionalOffices.Get(p => p.ProfessionalId == professionalId && p.Active, includeProperties: "ContactLocationPictures,City");
            SetMarkers(offices);           
            return PartialView(offices);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult ProfessionalOfficeLocations()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            ViewBag.ProfessionalId = professionalId;
            
            var offices = DataBase.ProfessionalOffices.Get(p => p.ProfessionalId == professionalId && p.Active);            
            SetMarkers(offices);
            return PartialView("ProfessionalOfficesMapMarker", offices);
        }        

        [ChildAction(Order = 1),Authorize(UserType.Professional, UserType.Assistant, Order = 2)]
        public ActionResult EditOffice(int? contactLocationId = null)
        {            
            if (contactLocationId == null)
            {
                ViewBag.CityId = DataBase.Cities.ToSelectList();
                Models.Pro.ProfessionalOffice office = new Models.Pro.ProfessionalOffice();
                office.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                return PartialView(office);
            }
            else
            {
                Models.Pro.ProfessionalOffice office = DataBase.ProfessionalOffices.GetByID(contactLocationId.Value);
                ViewBag.CitiesSelectedList = DataBase.Cities.ToSelectList(selected: p => p.CityId == office.CityId);
                return PartialView(office);
            }            
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditOffice(Models.Pro.View.ProfessionalOffice viewProfessionalOffice)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                bool insert = viewProfessionalOffice.ContactLocationId == 0;
                Models.Pro.ProfessionalOffice professionalOffice = null;
                if (insert)
                {
                    professionalOffice = new Models.Pro.ProfessionalOffice();
                    professionalOffice.ProfessionalId = viewProfessionalOffice.ProfessionalId;
                    professionalOffice.ContactId = viewProfessionalOffice.ProfessionalId;
                    professionalOffice.Active = true;
                }
                else
                {
                    professionalOffice = DataBase.ProfessionalOffices.GetByID(viewProfessionalOffice.ContactLocationId);
                }

                professionalOffice.CityId = viewProfessionalOffice.CityId;

                professionalOffice.Description = viewProfessionalOffice.Description;
                professionalOffice.Address = viewProfessionalOffice.Address;
                professionalOffice.References = viewProfessionalOffice.References;
                professionalOffice.DefaultPhoneNumber = viewProfessionalOffice.DefaultPhoneNumber;
                professionalOffice.DefaultPhoneExtension = viewProfessionalOffice.DefaultPhoneExtension;
                professionalOffice.PhoneNumber01 = viewProfessionalOffice.PhoneNumber01;
                professionalOffice.PhoneExtension01 = viewProfessionalOffice.PhoneExtension01;
               
                var city = DataBase.Cities.Get(p => p.CityId == viewProfessionalOffice.CityId, includeProperties: "State").Single();

                professionalOffice.StateId = city.State.StateId;
                professionalOffice.CountryId = city.State.CountryId;

                if (insert)
                    DataBase.ProfessionalOffices.Insert(professionalOffice);
                else
                    DataBase.ProfessionalOffices.Update(professionalOffice);

                DataBase.Save();
                //}
            }
            catch
            {
                return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error } };
            }
            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult SetLocation(int contactLocationId, int markerIndex)
        {
            var professionalOffice = DataBase.ProfessionalOffices.GetByID(contactLocationId);
            ViewBag.SetClientLocation = false;
            if (professionalOffice.Latitude == null || professionalOffice.Longitude == null)
            {
                professionalOffice.Latitude = Sic.Web.Mvc.Session.DefaultLatitude;
                professionalOffice.Longitude = Sic.Web.Mvc.Session.DefaultLongitude;

                ViewBag.SetClientLocation = true;
            }

            professionalOffice.LatitudeString = professionalOffice.Latitude.ToString();
            professionalOffice.LongitudeString = professionalOffice.Longitude.ToString();

            ViewBag.MapEdit = true;
            professionalOffice.MarkerIndex = markerIndex;

            return PartialView(professionalOffice);
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult SetLocation(int contactLocationId, string latitudeString, string longitudeString)
        {
            try
            {
                var professionalOffice = DataBase.ProfessionalOffices.GetByID(contactLocationId);

                if (!string.IsNullOrEmpty(latitudeString))
                {
                    professionalOffice.Latitude = Convert.ToDouble(latitudeString.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
                    professionalOffice.Longitude = Convert.ToDouble(longitudeString.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
                }
                else
                {
                    if (professionalOffice.Latitude != null)
                    {
                        professionalOffice.Latitude = professionalOffice.Latitude;
                        professionalOffice.Longitude = professionalOffice.Longitude;
                    }
                }

                DataBase.ProfessionalOffices.Update(professionalOffice);

                DataBase.Save();

                return new JsonResult() { Data = new { Success = true, Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
            }
            catch (Exception)
            {
                return new JsonResult() { Data = new { Success = false, Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error } };
            }            
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult OfficePictures(int contactLocationId)
        {
            return View(DataBase.ProfessionalOffices.Get(p => p.ContactLocationId == contactLocationId, includeProperties: "ContactLocationPictures").Single());
        }

        #endregion

        #region School

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Schools()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId,
                includeProperties: "Contact,ProfessionalSchools").SingleOrDefault();            
            ViewBag.ProfessionalId = professionalId;

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(professional);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult SchoolIndex()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            ViewBag.ProfessionalId = professionalId;
            var schools = DataBase.ProfessionalSchools.
                Get(p => p.ProfessionalId == professionalId);

            return PartialView(schools);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult EditSchool(int? professionalSchoolId = null, int? institutionId = null, string institutionName = null)
        {            
            if (professionalSchoolId == null)
            {
                ProfessionalSchool school = new ProfessionalSchool();
                if (institutionId.HasValue && institutionId!=0)
                    school.InstitutionId = institutionId.Value;
                if (!string.IsNullOrEmpty(institutionName))
                    school.Name = institutionName;

                school.ProfessionalId = Sic.Apollo.Session.ProfessionalId;

                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(school);
            }
            else if (professionalSchoolId != null)
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(DataBase.ProfessionalSchools.GetByID(professionalSchoolId.Value));
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return PartialView(new ProfessionalSchool());
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditSchool(ProfessionalSchool professionalSchool)
        {
            try
            {
                bool insert = professionalSchool.ProfessionalSchoolId == 0;
                ProfessionalSchool professionalSchoolUpdate = null;
                if (insert)
                {
                    professionalSchoolUpdate = new ProfessionalSchool();
                }
                else
                {
                    professionalSchoolUpdate = DataBase.ProfessionalSchools.GetByID(professionalSchool.ProfessionalSchoolId);
                }

                professionalSchoolUpdate.Description = professionalSchool.Description;
                professionalSchoolUpdate.Name = professionalSchool.Name;
                professionalSchoolUpdate.InstitutionId = professionalSchool.InstitutionId;
                professionalSchoolUpdate.StartYear = professionalSchool.StartYear;
                professionalSchoolUpdate.EndYear = professionalSchool.EndYear;
                professionalSchoolUpdate.ProfessionalId = professionalSchool.ProfessionalId;


                if (insert)
                    DataBase.ProfessionalSchools.Insert(professionalSchoolUpdate);
                else
                    DataBase.ProfessionalSchools.Update(professionalSchoolUpdate);

                DataBase.Save();
            }
            catch
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error } };
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeleteSchool(int professionalSchoolId)
        {            
            DataBase.ProfessionalSchools.Delete(professionalSchoolId);
            DataBase.Save();

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        #endregion

        #region Community

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Communities()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId,
                includeProperties: "Contact,ProfessionalCommunities").SingleOrDefault();
            ViewBag.ProfessionalId = professionalId;

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(professional);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult CommunityIndex()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;

            ViewBag.ProfessionalId = professionalId;

            var communities = DataBase.ProfessionalCommunities.
                Get(p => p.ProfessionalId == professionalId);

            return PartialView(communities);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult EditCommunity(int? professionalCommunityId = null, int? institutionId = null, string institutionName = null)
        {
            if (professionalCommunityId == null)
            {
                ProfessionalCommunity community = new ProfessionalCommunity();
                if (institutionId.HasValue && institutionId != 0)
                    community.InstitutionId = institutionId.Value;
                if (!string.IsNullOrEmpty(institutionName))
                    community.Name = institutionName;

                community.ProfessionalId = Sic.Apollo.Session.ProfessionalId;

                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(community);
            }
            else if (professionalCommunityId != null)
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(DataBase.ProfessionalCommunities.GetByID(professionalCommunityId.Value));
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return PartialView(new ProfessionalCommunity());
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditCommunity(ProfessionalCommunity professionalCommunity)
        {
            try
            {
                bool insert = professionalCommunity.ProfessionalCommunityId == 0;
                ProfessionalCommunity professionalCommunityUpdate = null;
                if (insert)
                {
                    professionalCommunityUpdate = new ProfessionalCommunity();
                }
                else
                {
                    professionalCommunityUpdate = DataBase.ProfessionalCommunities.GetByID(professionalCommunity.ProfessionalCommunityId);
                }

                professionalCommunityUpdate.Description = professionalCommunity.Description;
                professionalCommunityUpdate.Name = professionalCommunity.Name;
                professionalCommunityUpdate.InstitutionId = professionalCommunity.InstitutionId;
                professionalCommunityUpdate.StartYear = professionalCommunity.StartYear;
                professionalCommunityUpdate.EndYear = professionalCommunity.EndYear;
                professionalCommunityUpdate.ProfessionalId = professionalCommunity.ProfessionalId;

                if (insert)
                    DataBase.ProfessionalCommunities.Insert(professionalCommunityUpdate);
                else
                    DataBase.ProfessionalCommunities.Update(professionalCommunityUpdate);

                DataBase.Save();
            }
            catch
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error } };
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeleteCommunity(int professionalCommunityId)
        {
            DataBase.ProfessionalCommunities.Delete(professionalCommunityId);
            DataBase.Save();

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        #endregion

        #region Experience

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Experiences()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == professionalId,
                includeProperties: "Contact,ProfessionalExperiences").SingleOrDefault();
            ViewBag.ProfessionalId = professionalId;

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(professional);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult ExperienceIndex()
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;

            ViewBag.ProfessionalId = professionalId;

            var experiences = DataBase.ProfessionalExperiences.
                Get(p => p.ProfessionalId == professionalId);

            return PartialView(experiences);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult EditExperience(int? professionalExperienceId = null, int? institutionId = null, string institutionName = null)
        {
            if (professionalExperienceId == null)
            {
                ProfessionalExperience experience = new ProfessionalExperience();
                if (institutionId.HasValue && institutionId != 0)
                    experience.InstitutionId = institutionId.Value;
                if (!string.IsNullOrEmpty(institutionName))
                    experience.Name = institutionName;

                experience.ProfessionalId = Sic.Apollo.Session.ProfessionalId;

                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(experience);
            }
            else if (professionalExperienceId != null)
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return PartialView(DataBase.ProfessionalExperiences.GetByID(professionalExperienceId.Value));
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return PartialView(new ProfessionalExperience());
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditExperience(ProfessionalExperience professionalExperience)
        {
            try
            {
                bool insert = professionalExperience.ProfessionalExperienceId == 0;
                ProfessionalExperience professionalExperienceUpdate = null;
                if (insert)
                {
                    professionalExperienceUpdate = new ProfessionalExperience();
                }
                else
                {
                    professionalExperienceUpdate = DataBase.ProfessionalExperiences.GetByID(professionalExperience.ProfessionalExperienceId);
                }

                professionalExperienceUpdate.Description = professionalExperience.Description;
                professionalExperienceUpdate.Name = professionalExperience.Name;
                professionalExperienceUpdate.InstitutionId = professionalExperience.InstitutionId;
                professionalExperienceUpdate.StartYear = professionalExperience.StartYear;
                professionalExperienceUpdate.EndYear = professionalExperience.EndYear;
                professionalExperienceUpdate.ProfessionalId = professionalExperience.ProfessionalId;

                if (insert)
                    DataBase.ProfessionalExperiences.Insert(professionalExperienceUpdate);
                else
                    DataBase.ProfessionalExperiences.Update(professionalExperienceUpdate);

                DataBase.Save();
            }
            catch
            {
                ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

                return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, MessageType = Sic.Constants.MessageType.Error } };
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeleteExperience(int professionalExperienceId)
        {
            DataBase.ProfessionalExperiences.Delete(professionalExperienceId);
            DataBase.Save();

            return new JsonResult() { Data = new { Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
        }

        #endregion

        #region Maps


        [ChildAction]
        public ActionResult ProfessionalOfficeMapSync(IEnumerable<Sic.Apollo.Models.Pro.View.Professional> professionals, int size = 3)
        {
            ViewBag.Size = size;
            return PartialView("_ProfessionalOfficeMapSync", professionals.Where(p => p.Latitude != null && p.Longitude != null));
        }

        #endregion

        #region Scheduler

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult OfficesScheduling(int contactLocationId)
        {
            int professionalId = Sic.Apollo.Session.ProfessionalId;
            return PartialView(GetOfficesScheduling(professionalId, contactLocationId));
        }

        private IEnumerable<ProfessionalOfficeSchedule> GetOfficesScheduling(int professionalId, int contactLocationId)
        {
            return DataBase.ProfessionalOfficeSchedules.Get(p => p.ProfessionalOffice.ProfessionalId == professionalId && 
                p.ContactLocationId == contactLocationId && p.Active);                
        }

        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult EditOfficeSchedule(int? contactLocationId = null,int? professionalOfficeScheduleId = null)
        {
            Models.Pro.ProfessionalOfficeSchedule schedule = new ProfessionalOfficeSchedule();
            if(professionalOfficeScheduleId == null)
            {                
                schedule = new Models.Pro.ProfessionalOfficeSchedule();
                schedule.ContactLocationId = contactLocationId.Value;
                schedule.StartTime = new DateTime(1900, 1, 1, 9, 0, 0);
                schedule.EndTime = new DateTime(1900, 1, 1, 17, 0, 0);
                schedule.ValidityStartDate = Sic.Web.Mvc.Session.CurrentDateTime.Date;       
                schedule.IndefiniteEndDate = true;
                schedule.AppointmentDuration = 30;
                schedule.ForEachWeek = 1;
                schedule.Monday = true;
                schedule.Tuesday = true;
                schedule.Wednesday = true;
                schedule.Thursday = true;
                schedule.Friday = true;
                schedule.Active = true;                
                schedule.ProfessionalOffice = DataBase.ProfessionalOffices.Get(p=>p.ContactLocationId == contactLocationId.Value, includeProperties:"Professional").SingleOrDefault();
            }
            else
            {
                schedule = DataBase.ProfessionalOfficeSchedules.Get(p=>p.ProfessionalOfficeScheduleId == professionalOfficeScheduleId, includeProperties: "ProfessionalOffice").SingleOrDefault();
            }

            schedule.ValidityStartDateString = schedule.ValidityStartDate.ToString(Sic.Core.Constants.FormatString.DefaultEditorDateFormat);
            if(schedule.ValidityEndDate.HasValue)
                schedule.ValidityEndDateString = schedule.ValidityEndDate.Value.ToString(Sic.Core.Constants.FormatString.DefaultEditorDateFormat);

            return PartialView(schedule);
        }

        [HttpPost]
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public JsonResult EditOfficeSchedule(ProfessionalOfficeSchedule professionalOfficeSchedule)
        {
            try
            {
                #region Validity DateTime

                DateTime resultDateTime;
                if (!DateTime.TryParseExact(professionalOfficeSchedule.ValidityStartDateString,
                        Sic.Core.Constants.FormatString.DefaultEditorDateFormat,
                        System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.None, out resultDateTime))
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Success = false,
                            Message = string.Format(Sic.Apollo.Resources.Resources.ValidationFieldDataTypeNameFor,
                                Sic.Apollo.Resources.Resources.LabelForScheduleStartDate),
                            MessageType = Sic.Constants.MessageType.Error
                        }
                    };
                }

                if (!string.IsNullOrWhiteSpace(professionalOfficeSchedule.ValidityEndDateString))
                {
                    if (!DateTime.TryParseExact(professionalOfficeSchedule.ValidityEndDateString,
                            Sic.Core.Constants.FormatString.DefaultEditorDateFormat,
                            System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.None, out resultDateTime))
                    {
                        return new JsonResult()
                        {
                            Data = new
                            {
                                Success = false,
                                Message = string.Format(Sic.Apollo.Resources.Resources.ValidationFieldDataTypeNameFor,
                                    Sic.Apollo.Resources.Resources.LabelForScheduleEndDate),
                                MessageType = Sic.Constants.MessageType.Error
                            }
                        };
                    }
                }                    

                if (professionalOfficeSchedule.StartTime > professionalOfficeSchedule.EndTime)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Success = false,
                            Message = string.Format(Sic.Apollo.Resources.Resources.ValidationFieldComparer,
                                Sic.Apollo.Resources.Resources.LegendForScheduleStartTime, "<", Sic.Apollo.Resources.Resources.LegendForScheduleEndTime),
                            MessageType = Sic.Constants.MessageType.Error
                        }
                    };
                }

                if (professionalOfficeSchedule.ValidityEndDate.HasValue && professionalOfficeSchedule.ValidityEndDate.Value < professionalOfficeSchedule.ValidityStartDate)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Success = false,
                            Message = Sic.Apollo.Resources.Resources.ValidationForDateRangeInvalid,
                            MessageType = Sic.Constants.MessageType.Error
                        }
                    };
                    //ModelState.AddModelError("ValidityEndDateString", Sic.Apollo.Resources.Resources.ValidationForDateRangeInvalid);
                }

                if (!professionalOfficeSchedule.IndefiniteEndDate && !professionalOfficeSchedule.ValidityEndDate.HasValue)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Success = false,
                            Message = Sic.Apollo.Resources.Resources.ValidationForScheduleValidityEndDate,
                            MessageType = Sic.Constants.MessageType.Error
                        }
                    };
                    //ModelState.AddModelError("ValidityEndDateString", Sic.Apollo.Resources.Resources.ValidationForScheduleValidityEndDate);
                }

                #endregion

                if (professionalOfficeSchedule.IndefiniteEndDate)
                    professionalOfficeSchedule.ValidityEndDate = null;

                bool insert = professionalOfficeSchedule.ProfessionalOfficeScheduleId == 0;
                ProfessionalOfficeSchedule professionalOfficeScheduleUpdate = null;

                if (insert)
                {
                    professionalOfficeScheduleUpdate = new ProfessionalOfficeSchedule();
                    professionalOfficeScheduleUpdate.Active = true;
                    professionalOfficeScheduleUpdate.ContactLocationId = professionalOfficeSchedule.ContactLocationId;
                    DataBase.ProfessionalOfficeSchedules.Insert(professionalOfficeScheduleUpdate);
                    //UpdateModel(professionalOfficeScheduleUpdate);                
                }
                else
                {
                    professionalOfficeScheduleUpdate = DataBase.ProfessionalOfficeSchedules.GetByID(professionalOfficeSchedule.ProfessionalOfficeScheduleId);
                    DataBase.ProfessionalOfficeSchedules.Update(professionalOfficeScheduleUpdate);
                    //UpdateModel(professionalOfficeScheduleUpdate);
                }

                professionalOfficeScheduleUpdate.AppointmentDuration = professionalOfficeSchedule.AppointmentDuration;
                professionalOfficeScheduleUpdate.EndTime = professionalOfficeSchedule.EndTime;
                professionalOfficeScheduleUpdate.ForEachWeek = professionalOfficeSchedule.ForEachWeek;
                professionalOfficeScheduleUpdate.Friday = professionalOfficeSchedule.Friday;
                professionalOfficeScheduleUpdate.IndefiniteEndDate = professionalOfficeSchedule.IndefiniteEndDate;
                professionalOfficeScheduleUpdate.Monday = professionalOfficeSchedule.Monday;
                professionalOfficeScheduleUpdate.Saturday = professionalOfficeSchedule.Saturday;
                professionalOfficeScheduleUpdate.StartTime = professionalOfficeSchedule.StartTime;
                professionalOfficeScheduleUpdate.Sunday = professionalOfficeSchedule.Sunday;
                professionalOfficeScheduleUpdate.Thursday = professionalOfficeSchedule.Thursday;
                professionalOfficeScheduleUpdate.Tuesday = professionalOfficeSchedule.Tuesday;
                professionalOfficeScheduleUpdate.ValidityEndDate = professionalOfficeSchedule.ValidityEndDate;
                professionalOfficeScheduleUpdate.ValidityStartDate = professionalOfficeSchedule.ValidityStartDate;                
                professionalOfficeScheduleUpdate.Wednesday = professionalOfficeSchedule.Wednesday;

                professionalOfficeScheduleUpdate.ValidityStartDateString = professionalOfficeScheduleUpdate.ValidityStartDate.ToString(Sic.Core.Constants.FormatString.DefaultEditorDateFormat);
                if(professionalOfficeScheduleUpdate.ValidityEndDate.HasValue)
                    professionalOfficeScheduleUpdate.ValidityEndDateString = professionalOfficeScheduleUpdate.ValidityEndDate.Value.ToString(Sic.Core.Constants.FormatString.DefaultEditorDateFormat);

                if (insert)
                    DataBase.Save();//Generate ID

                DataBase.Appointments.GenerateAppointments(professionalOfficeScheduleUpdate);
                DataBase.Save();

                return new JsonResult()
                {
                    Data = new
                    {
                        Success = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveOk,
                        MessageType = Sic.Constants.MessageType.Success
                    }
                };
                //if (professionalOfficeSchedule.ProfessionalOffice==null)
                //    professionalOfficeSchedule.ProfessionalOffice = DataBase.ProfessionalOffices.GetByID(professionalOfficeSchedule.ContactLocationId);
            }
            catch(Exception)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure,
                        MessageType = Sic.Constants.MessageType.Error
                    }
                };
            }
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult DeleteOfficeSchedule(int professionalOfficeScheduleId)
        {
            try
            {
                DataBase.Appointments.DeleteScheduleAppointment(professionalOfficeScheduleId);
                DataBase.ProfessionalOfficeSchedules.Delete(professionalOfficeScheduleId);
                DataBase.Save();
                return new JsonResult() { Data = new { Success = true, 
                    Message = Sic.Apollo.Resources.Resources.MessageForSaveOk, MessageType = Sic.Constants.MessageType.Success } };
            }
            catch(Exception)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure, 
                        MessageType = Sic.Constants.MessageType.Error } };
            }
        }
        #endregion

        #region Confirmation

        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalConfirmation()
        {
            return View(DataBase.Professionals.GetProfessionalsPendingConfirmation());
        }

        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalConfirm(Sic.Apollo.Models.Pro.View.Professional professional)
        {
            return View(professional);
        }

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalConfirm(int professionalId)
        {
            var user = DataBase.Users.Get(p => p.UserId == professionalId, includeProperties:"Contact").Single();
            user.State = (int)UserState.Active;
            user.ConfirmationDate = Sic.Web.Mvc.Session.CurrentDateTime;
            DataBase.Users.Update(user);

            DataBase.Save();

            Mail.SendProfessionalConfirmed(user.Contact.FullName2, user.Contact.Email, (Gender)user.Contact.Gender);

            return RedirectToAction("ProfessionalConfirmed", new { professionalId = professionalId });
        }

        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalConfirmed(int professionalId)
        {
            ViewBag.ProfessionalId = professionalId;

            return View();
        }

        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalSuspend(Sic.Apollo.Models.Pro.View.Professional professional)
        {
            return View(professional);
        }

        [HttpPost]
        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalSuspend(int professionalId)
        {
            var user = DataBase.Users.Get(p => p.UserId == professionalId).Single();
            user.State = (int)UserState.Suspended;
            DataBase.Users.Update(user);

            DataBase.Save();

            return RedirectToAction("ProfessionalSuspended", new { professionalId = professionalId });
        }

        [Authorize(UserType.Administrator)]
        public ActionResult ProfessionalSuspended(int professionalId)
        {
            ViewBag.ProfessionalId = professionalId;

            return View();
        }

        #endregion Confirmation

        #region Team

        [Authorize(UserType.Professional)]
        public ActionResult Team()
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;
            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalTeam.TeamUser.Contact").SingleOrDefault();
            return View(professional);
        }

        [Authorize(UserType.Professional)]
        public ActionResult CreateTeam()
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;
            User user = new User();            
            return View("User", user);
        }

        [Authorize(UserType.Professional)]
        [HttpPost]
        public ActionResult CreateTeam(User user)
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;

            user.Validate(this.ModelState);

            if (ModelState.IsValid)
            {
                try
                {
                    ProfessionalTeam professionalTeam = new ProfessionalTeam();
                    professionalTeam.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                    professionalTeam.TeamUser = user;
                    professionalTeam.Active = true;
                    user.RegisterDate = Sic.Web.Mvc.Session.CurrentDateTime;
                    user.Contact.Email = user.LogonName;
                    user.State = (int)UserState.Active;

                    DataBase.Contacts.Insert(professionalTeam.TeamUser.Contact);
                    DataBase.Users.Insert(professionalTeam.TeamUser);
                    DataBase.ProfessionalTeams.Insert(professionalTeam);

                    Sic.Apollo.Models.Pro.Customer customer = new Sic.Apollo.Models.Pro.Customer();
                    customer.Contact = professionalTeam.TeamUser.Contact;
                    DataBase.Customers.Insert(customer);

                    DataBase.Save();

                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
                    ViewBag.MessageType = Sic.Constants.MessageType.Success;

                    return RedirectToAction("Team");
                }
                catch
                {
                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                    ViewBag.MessageType = Sic.Constants.MessageType.Error;
                }
            }
            
            return View("User", user);            
        }

        [Authorize(UserType.Professional)]
        public ActionResult EditTeam(int professionalTeamId)
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;
            ProfessionalTeam profesionalTeam = DataBase.ProfessionalTeams.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                && p.ProfessionalTeamId == professionalTeamId,includeProperties:"TeamUser.Contact").SingleOrDefault();

            EditUserContact edit = new EditUserContact()
            {
                Contact = profesionalTeam.TeamUser.Contact,
                LogonName = profesionalTeam.TeamUser.LogonName,
                ProfessionalTeamId = profesionalTeam.ProfessionalTeamId,                
                Type = profesionalTeam.TeamUser.Type,
                UserId = profesionalTeam.TeamUser.UserId
            };
            return View("EditUser", edit);
        }

        [Authorize(UserType.Professional)]
        [HttpPost]
        public ActionResult EditTeam(EditUserContact user)
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;

            ProfessionalTeam professionalTeamUpdate = DataBase.ProfessionalTeams.Get(p => p.ProfessionalTeamId == user.ProfessionalTeamId,
            includeProperties:"TeamUser.Contact").SingleOrDefault();

            user.Validate(ModelState);
            this.TryUpdateModel(professionalTeamUpdate.TeamUser);

            ModelState.Remove("ConfirmedPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    professionalTeamUpdate.TeamUser.Contact.Email = professionalTeamUpdate.TeamUser.LogonName;
                    professionalTeamUpdate.TeamUser.ConfirmedPassword = professionalTeamUpdate.TeamUser.Password;
                    DataBase.ProfessionalTeams.Update(professionalTeamUpdate);
                    DataBase.Users.Update(professionalTeamUpdate.TeamUser);
                    DataBase.Contacts.Update(professionalTeamUpdate.TeamUser.Contact);
                    DataBase.Save();

                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
                    ViewBag.MessageType = Sic.Constants.MessageType.Success;

                    return RedirectToAction("Team");

                }
                catch
                {
                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                    ViewBag.MessageType = Sic.Constants.MessageType.Error;
                }
            }
            return View("EditUser", user);            
        }

        [Authorize(UserType.Professional)]
        [HttpPost]
        public ActionResult DeleteTeam(int professionalTeamId)
        {
            ViewBag.ProfessionalOption = ProfessionalOption.Team;            

            try
            {
                ProfessionalTeam profesionalTeam = DataBase.ProfessionalTeams.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId
                   && p.ProfessionalTeamId == professionalTeamId, includeProperties: "TeamUser").SingleOrDefault();

                //DataBase.ProfessionalTeams.Delete(profesionalTeam);
                profesionalTeam.Active = false;
                profesionalTeam.TeamUser.Type = (int)UserType.Customer;

                DataBase.ProfessionalTeams.Update(profesionalTeam);
                DataBase.Users.Update(profesionalTeam.TeamUser);
                DataBase.Save();

                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
                ViewBag.MessageType = Sic.Constants.MessageType.Success;

                return RedirectToAction("Team");
            }
            catch
            {
                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                ViewBag.MessageType = Sic.Constants.MessageType.Error;
            }

            var professional = DataBase.Professionals.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
            includeProperties: "Contact,ProfessionalTeam.TeamUser.Contact").SingleOrDefault();

            return View("Team", professional);
        }


        #endregion

        #region Others

        [ChildAction]
        public ActionResult ProfessionalRate(int professionalId)
        {
            ViewBag.ProfessionalId = professionalId;

            return PartialView();
        }

        [ChildAction]
        public ActionResult ProfessionalRateList(int professionalId, bool resume = false)
        {
            ViewBag.Resume = resume;

            return PartialView(DataBase.AppointmentTransactions.GetRateAppointments(professionalId, resume));
        }
        
        [ChildAction]
        public ActionResult SpecializationList(Models.Pro.Professional professional)
        {
            return PartialView(professional);
        }

        [ChildAction]
        public ActionResult ProfessionalCard(int? professionalId = null, int? contactLocationId = null, int? specializationId = null, bool showBookOnline = true, bool linkAvailable = true)
        {
            ViewBag.ShowBookOnline = showBookOnline;
            ViewBag.LinkAvailable = linkAvailable;
            
            if (contactLocationId == 0) contactLocationId = null;
            if (specializationId == 0) specializationId = null;

            if (contactLocationId == null && specializationId == null)
                return PartialView(DataBase.Professionals.GetProfessionals(professionalId).First());
            else 
                return PartialView(DataBase.Professionals.GetProfessionals(professionalId: professionalId, contactLocationId: contactLocationId, specialityId: specializationId).First());
        }        

        protected override void Dispose(bool disposing)
        {
            DataBase.Dispose();
            base.Dispose(disposing);
        }

        #endregion

    }
}