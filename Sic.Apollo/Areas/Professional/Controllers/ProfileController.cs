using Sic.Apollo.Models;
using Sic.Apollo.Models.Pro;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class ProfileController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        [ChildAction]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Options(int option)
        {
            ViewBag.ProfessionalOption = ((ProfessionalOption)option);
            Models.Pro.View.ProfessionalSummary summary = DataBase.Professionals.GetPofessionalSummary(Sic.Apollo.Session.ProfessionalId);
            return PartialView("_ProfileOptions", summary);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Profile()
        {
            int userId = Sic.Apollo.Session.ProfessionalId;
            Models.Pro.Professional professional = DataBase.Professionals.Get(p => p.ProfessionalId == userId,
            includeProperties: "Contact,ProfessionalSpecializations").Single();

            professional.Contact.AssingDateParts();

            FillProfileData(professional);

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(professional);
        }

        private void FillProfileData(Models.Pro.Professional professional)
        {
            MultiSelectList specializationList = DataBase.Specializations.
                Get(p => p.Active).OrderBy(p => p.Name).ToMultiSelectList(selectedValues: professional.ProfessionalSpecializations.Select(p => p.SpecializationId).ToArray(),
            valueMember: "Key", displayMember: "Name");
            ViewBag.SpecializationsList = specializationList;
        }

        [HttpPost]
        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult Profile(Models.Pro.Professional professional, int[] specializationMultiSelect)
        {
            Models.Pro.Professional professionalUpdate = null;
            try
            {
                professionalUpdate = DataBase.Professionals.Get(p => p.ProfessionalId == Sic.Apollo.Session.ProfessionalId,
                            includeProperties: "Contact,ProfessionalSpecializations").Single();

                TryUpdateModel(professionalUpdate);

                if (specializationMultiSelect == null)
                    specializationMultiSelect = new int[0];

                var deleteSpecializations = professionalUpdate.ProfessionalSpecializations.
                    Where(p => !specializationMultiSelect.Contains(p.SpecializationId)).ToList();

                foreach (var delete in deleteSpecializations)
                {
                    DataBase.ProfessionalSpecializations.Delete(delete);
                    professionalUpdate.ProfessionalSpecializations.Remove(delete);
                }

                foreach (int specializationId in specializationMultiSelect.Where(p =>
                    !professionalUpdate.ProfessionalSpecializations.Any(q => q.SpecializationId == p)))
                {
                    ProfessionalSpecialization profession = new ProfessionalSpecialization()
                    {
                        SpecializationId = specializationId,
                        ProfessionalId = professional.ProfessionalId
                    };

                    professionalUpdate.ProfessionalSpecializations.Add(profession);
                    DataBase.ProfessionalSpecializations.Insert(profession);
                }

                professionalUpdate.Validate(ModelState);

                if (ModelState.IsValid)
                {
                    Models.Security.User userProfessional = DataBase.Users.GetByID(Sic.Apollo.Session.ProfessionalId);
                    userProfessional.LogonName = professionalUpdate.Contact.Email;
                    userProfessional.ConfirmedPassword = userProfessional.Password;

                    DataBase.Users.Update(userProfessional);

                    DataBase.Professionals.Update(professionalUpdate);
                    DataBase.Contacts.Update(professionalUpdate.Contact);
                    DataBase.Save();

                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
                    ViewBag.MessageType = Sic.Constants.MessageType.Success;
                }
                else
                {
                    ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForVerifyIncorrectData;
                    ViewBag.MessageType = Sic.Constants.MessageType.Warning;
                }
            }
            catch (Exception)
            {
                ViewBag.Message = Sic.Apollo.Resources.Resources.MessageForSaveFailure;
                ViewBag.MessageType = Sic.Constants.MessageType.Error;
            }
            finally
            {
                FillProfileData(professionalUpdate);
            }

            ViewBag.ProfessionalOption = ProfessionalOption.EditProfile;

            return View(professionalUpdate);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult ProfessionalProfilePicture()
        {
            return View();
        }
	}
}