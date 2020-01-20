using System;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Services;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private MemberService svc = new MemberService();

        public ActionResult Index()
        {
            var model = svc.GetGroups();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(Group model)
        {
            if (model.GroupId == 0)
            {
                model.CreatedDate = DateTime.Now;
            }

            svc.SaveGroup(model);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int groupId)
        {
            svc.DeleteGroup(groupId);

            return RedirectToAction("Index");
        }
    }
}