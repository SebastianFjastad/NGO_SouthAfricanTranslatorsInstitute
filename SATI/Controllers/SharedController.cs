using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Services;
using System.Web.Mvc;

namespace SATI.Controllers
{
    [Authorize]
    public class SharedController : Controller
    {
        MemberService svc = new MemberService();

        public ActionResult SaveLanguage(string memberId, int languageId)
        {
            svc.SaveMemberLanguage(ResolveMemberId(memberId), languageId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteLanguage(string memberId, int languageId)
        {
            svc.DeleteMemberLanguage(ResolveMemberId(memberId), languageId);

            if (User.IsInRole("Member"))
                return RedirectToAction("Languages", "Details", new { area = "Members" });
            else
                return RedirectToAction("Languages", "Members", new { memberId, area = "Admin" });
        }

        public JsonResult SaveAccreditations(int capabilityId, List<int> ids)
        {
            svc.SaveAccreditations(capabilityId, ids);
            return Json(new { success = true });
        }

        public JsonResult SaveSpecialisations(int capabilityId, List<int> ids)
        {
            svc.SaveSpecialisations(capabilityId, ids);
            return Json(new { success = true });
        }

        public JsonResult SaveIsAccredited(int capabilityId, bool isAccredited)
        {
            svc.SaveIsAccredited(capabilityId, isAccredited);
            return Json(new { sucess = true });
        }

        public PartialViewResult CapabilityDetails(int capabilityId, string memberId)
        {
            var capability = svc.GetCapability(capabilityId);
            var specialisations = svc.GetAllSpecialisations();
            var accreditations = svc.GetAllAccreditations();
            var model = new CapabilityDetailsViewModel(capability, ResolveMemberId(memberId), specialisations, accreditations);

            return PartialView("~/Views/Shared/Member/_CapabilityDetails.cshtml", model);
        }

        public JsonResult AddCapability(string memberId, int skillId, int fromLanguageId, int? toLanguageId)
        {
            //check if combination is valid
            if (!svc.CheckSkillsCombinationIsValid(ResolveMemberId(memberId), skillId, fromLanguageId, toLanguageId))
                return Json(new { ErrorMessage = "The Skill and Language combination already exists." }, JsonRequestBehavior.AllowGet);

            //save
            svc.SaveCapability(ResolveMemberId(memberId), new Capability
            {
                SkillId = skillId,
                FromId = fromLanguageId,
                ToId = toLanguageId
            });

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCapability(string memberId, int capabilityId)
        {
            svc.DeleteCapability(capabilityId);
            if (User.IsInRole("Member"))
                return RedirectToAction("Capabilities", "Details", new { area = "Members" });
            else
                return RedirectToAction("Capabilities", "Members", new { memberId, area = "Admin" });
        }

        private string ResolveMemberId(string memberId)
        {
            return !string.IsNullOrEmpty(memberId) ? memberId : User.Identity.GetUserId();
        }
    }
}