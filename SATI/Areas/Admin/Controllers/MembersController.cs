using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Models;
using SATI.Services;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        MemberService svc = new MemberService();

        public ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        #region Member Details
        public ActionResult Index(string memberId)
        {
            var model = new MemberViewModel
            {
                Titles = svc.GetMemberTitles(),
                Areas = svc.GetAreas(),
                Regions = svc.GetRegions(),
                Countries = svc.GetCountries()
            };

            if (!string.IsNullOrEmpty(memberId))
            {
                var foundUser = svc.GetMember(memberId);

                if (foundUser != null)
                {
                    model.User = foundUser;

                    if (foundUser.IsDeleted)
                        ViewBag.RemoveMemberId = memberId;
                }
            }

            ViewBag.MemberId = model.User?.Id;

            return View(model);
        }

        public JsonResult GetMember(string memberId)
        {
            var member = svc.GetMember(memberId);
            return Json(member, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Save(MemberViewModel model)
        {
            var hasUserId = string.IsNullOrEmpty(model.User.Id);
            var result = svc.SaveMember(model, UserManager);

            if (!result.Success)
            {
                TempData["Errors"] = result.Errors;
                if (model.IsGeneralDetailsSave)
                    return RedirectToAction("General", new { memberId = model.User.Id });
                else
                    return RedirectToAction("Index");
            }

            if (result == null)
            {
                TempData["SaveResult"] = false;
            }

            var shouldSendEmailOnCreate = Convert.ToBoolean(WebConfigurationManager.AppSettings["SendEmailOnMemberCreate"]);

            if (hasUserId && shouldSendEmailOnCreate)
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(model.User.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = model.User.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(
                    model.User.Id,
                    "Confirm your account",
                    "Hi " + model.User.FirstName + " " + model.User.FirstName + ",<br/>" +
                    "Your account has been created by SATI<br/>" +
                    "Your username is: " + model.User.Email + "<br/>" +
                    "Please create your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            return RedirectToAction("Index", new { memberId = result.User.Id });
        }

        public JsonResult Search(string term)
        {
            var membersResults = svc.SearchMembers(term);
            return Json(membersResults, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string memberId)
        {
            //use this same action to toggle IsDeleted status
            svc.DeleteUndeleteMember(memberId);
            return RedirectToAction("Index", new { memberId = memberId });
        }

        #endregion

        #region General Details

        public ActionResult General(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Index");
            }

            var model = new MemberViewModel
            {
                User = svc.GetMember(memberId),
                Countries = svc.GetCountries(),
                Groups = svc.GetGroups(),
                SocialMediaLinks = svc.GetSocialMediaLinks(),
                Languages = svc.GetLanguages(),
                IsGeneralDetailsSave = true
            };

            model.SortData();

            ViewBag.MemberId = model.User?.Id;

            return View(model);
        }

        #endregion

        #region Languages
        public ActionResult Languages(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Index");
            }

            var user = svc.GetMember(memberId);
            var languages = svc.GetAllLanguages();
            var model = new LanguagesViewModel(languages, user);
            ViewBag.MemberId = model.User?.Id;
            return View(model);
        }
        #endregion

        #region Language Capabilities

        public ActionResult Capabilities(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Index");
            }

            var member = svc.GetMemberWithCapabilities(memberId);
            var skills = svc.GetSkills();
            var model = new CapabilitiesViewModel(member, skills);
            ViewBag.MemberId = memberId;
            return View(model);
        }

        #endregion

        #region Payments

        public ActionResult Payments(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Index");
            }

            var model = new PaymentsViewModel(svc.GetPayments(memberId))
            {
                PaymentCategories = svc.GetAllPaymentCategories(),
                PaymentMethods = svc.GetAllPaymentMethods(),
                MemberId = memberId
            };
            ViewBag.MemberId = memberId;
            return View(model);
        }

        [HttpPost]
        public ActionResult SavePayment(string memberId, int paymentCategoryId, DateTime paymentDate,
            int financialYear, int paymentMethodId, CardPaymentType cardPaymentType, string comment)
        {
            var payment = new Payment
            {
                PaymentCategoryId = paymentCategoryId,
                PaymentDate = paymentDate,
                FinancialYear = financialYear,
                PaymentMethodId = paymentMethodId,
                CardPaymentType = cardPaymentType,
                Comment = comment
            };

            svc.SavePayment(memberId, payment);

            return Json(new { success = true });
        }

        public ActionResult DeletePayment(int paymentId, string memberId)
        {
            var payment = svc.repo._db.Payments.Find(paymentId);
            if (payment != null)
                svc.repo._db.Payments.Remove(payment);

            return RedirectToAction("Payments", new { memberId = memberId });
        }
        #endregion

        #region Email
        public async Task<ActionResult> EmailNewClients(string[] emailAddresses = null)
        {
            //get all users
            var users = svc.GetAllMembers(emailAddresses);

            foreach (var user in users)
            {
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id,
                    "SATI Account Migration",
                    "Your account has been migrated to a new platform." +
                    "<br/><br/>To log in to your new account please recover your password by clicking <a href=\"" + callbackUrl + "\">here</a>" +
                    "<br/><br/>Kind regards," +
                    "<br/><br/>The SATI team");
            }

            return Content($"Emails have been sent to {users.Count} members");
        }
        #endregion
    }
}