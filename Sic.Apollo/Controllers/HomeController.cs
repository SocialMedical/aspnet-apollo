using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Web.Mvc.Utility;
using Sic.Apollo.Models;
using System.Web.Security;
using Sic.Web.Mvc;
using Sic.Apollo.Models.Pro;
using Sic.Apollo.Models.General;

namespace Sic.Apollo.Controllers
{
    public class HomeController : Sic.Web.Mvc.Controllers.BaseController
    {        
        // GET: /Home/

        ContextService db = new ContextService();
        
        [OutputCache(CacheProfile="Cache-Long-Any")]
        public ActionResult Index()
        {
            FillSearchCriteria(this,db);             
            return View();
        }

        [ChildAction]
        public ActionResult SearchCriteria(int? specializationId = null, int? cityId = null,
            int? insuranceInstitutionId = null, string professionalName = null, bool fullMode = false)
        {
            FillSearchCriteria(this,db,specializationId, cityId, insuranceInstitutionId, professionalName, true);
            return PartialView();
        }

        [ChildAction]
        public ActionResult SearchCriteriaResult(int? specializationId = null, int? cityId = null,
            int? insuranceInstitutionId = null, string professionalName = null)
        {
            FillSearchCriteria(this,db,specializationId, cityId, insuranceInstitutionId, professionalName, true);
            return PartialView();
        }         

        public static void FillSearchCriteria(Controller currentController,ContextService db,int? specializationId = null, int? cityId = null,
            int? insuranceInstitutionId = null, string professionalName = null,bool isResult = false)
        {
            IEnumerable<City> cities = db.Cities.Get().OrderBy(p=>p.DescriptionName);
            List<InsuranceInstitution> insuranceInstitution = db.InsuranceInstitutions.Get(p => p.Active).OrderBy(p => p.DescriptionName).ToList();
            List<Specialization> specializations = db.Specializations.Get(p => p.Active).OrderBy(p => p.DescriptionName).ToList();

            #region Specialization

            var defaultSpecialization = specializations.FirstOrDefault(p=>p.IsDefault);
            if(defaultSpecialization!=null)
                currentController.ViewBag.DefaultSpecializationId = defaultSpecialization.SpecializationId;
            if (specializationId == null)
            {
                if(isResult)
                    currentController.ViewBag.SpecializationSelectedList = specializations.ToSelectList(s => s.IsDefault);
                else
                    currentController.ViewBag.SpecializationSelectedList = specializations.ToSelectList();
                currentController.ViewBag.ProfessionName = defaultSpecialization.ProfessionInPlural;           
            }
            else
            {
                currentController.ViewBag.SpecializationSelectedList = specializations.ToSelectList(s => s.SpecializationId == specializationId.Value);
                currentController.ViewBag.ProfessionName = specializations.SingleOrDefault(p => p.SpecializationId == specializationId).ProfessionInPlural;
            }

            #endregion

            #region Insurance

            if (insuranceInstitutionId == null)            
                currentController.ViewBag.InsuranceInstitutionSelectedList = insuranceInstitution.ToSelectList();            
            else            
                currentController.ViewBag.InsuranceInstitutionSelectedList = insuranceInstitution.ToSelectList(p=>p.InstitutionId == insuranceInstitutionId.Value);            

            #endregion

            #region city

            var defaultCity = cities.FirstOrDefault(p => p.IsDefault);
            if (defaultCity != null)
                currentController.ViewBag.DefaultCityId = defaultCity.CityId;  

            if (cityId == null)
            {
                if(isResult)
                    currentController.ViewBag.CitySelectedList = cities.ToSelectList(p=>p.IsDefault);
                else
                    currentController.ViewBag.CitySelectedList = cities.ToSelectList();

                currentController.ViewBag.CityName = defaultCity.Name;
            }
            else
            {
                currentController.ViewBag.CitySelectedList = cities.ToSelectList(p=>p.CityId == cityId.Value);
                currentController.ViewBag.CityName = cities.SingleOrDefault(p=>p.CityId == cityId).Name;
            }

            #endregion

            currentController.ViewBag.Specializations = specializations.OrderBy(p => p.Priority);
            currentController.ViewBag.InsuranceInstitutions = insuranceInstitution.OrderBy(p => p.Priority);
        }


        [OutputCache(CacheProfile = "Cache-Long-Any")]
        public ActionResult ChangeCulture()
        {
            ContextService db = new ContextService();            
            return View(db.Countries.Get());
        }

        public ActionResult Gallery()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetValidCulture(culture);
            
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.HttpOnly = false; // Not accessible by JS.
                cookie.Value = culture;
                cookie.Expires = Sic.Web.Mvc.Session.CurrentDateTime.AddYears(1);
            }

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");
        }
    }
}
