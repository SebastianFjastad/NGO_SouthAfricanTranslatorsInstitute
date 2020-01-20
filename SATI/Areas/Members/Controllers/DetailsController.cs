using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SATI.Areas.Admin.Models;
using SATI.Models;
using SATI.Services;
using System.Web;
using System.Web.Mvc;

namespace SATI.Areas.Members.Controllers
{
    [Authorize(Roles = "Member")]
    public class DetailsController : Controller
    {
        private MemberService _svc = new MemberService();
        public ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public ActionResult Index()
        {

            var model = new MemberViewModel
            {
                Titles = _svc.GetMemberTitles(),
                Areas = _svc.GetAreas(),
                Regions = _svc.GetRegions(),
                Countries = _svc.GetCountries(),
                User = _svc.GetMember(User.Identity.GetUserId())
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(MemberViewModel model)
        {
            var result = _svc.SaveMember(model, UserManager);

            if (!result.Success)
                TempData["Errors"] = result.Errors;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(MemberViewModel model)
        {
            return View();
        }

        public ActionResult General()
        {
            var model = new MemberViewModel
            {
                User = _svc.GetMember(User.Identity.GetUserId()),
                Countries = _svc.GetCountries(),
                Groups = _svc.GetGroups(),
                SocialMediaLinks = _svc.GetSocialMediaLinks(),
                Languages = _svc.GetLanguages(),
                IsGeneralDetailsSave = true
            };
            model.SortData();

            return View(model);
        }

        public ActionResult Capabilities()
        {
            var member = _svc.GetMemberWithCapabilities(User.Identity.GetUserId());
            var skills = _svc.GetSkills();
            var model = new CapabilitiesViewModel(member, skills);
            return View(model);
        }

        public ActionResult Languages()
        {
            var user = _svc.GetMember(User.Identity.GetUserId());
            var languages = _svc.GetAllLanguages();
            var model = new LanguagesViewModel(languages, user);
            ViewBag.MemberId = model.User?.Id;
            return View(model);
        }

        public ActionResult Payments()
        {
            var memberId = User.Identity.GetUserId();
            var model = new PaymentsViewModel(_svc.GetPayments(memberId));
            return View(model);
        }
    }
}