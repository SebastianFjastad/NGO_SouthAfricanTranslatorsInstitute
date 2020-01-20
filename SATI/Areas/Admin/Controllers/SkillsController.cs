using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Repositories;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillsController : Controller
    {
        private AdminRepository repo = new AdminRepository();

        public ActionResult Index()
        {
            var model = repo.Where<Skill>(s => !s.IsDeleted).OrderBy(s => s.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(Skill model)
        {
            repo.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var skillToDelete = repo.Single<Skill>(s => s.SkillId == id);
            repo.SoftDelete(skillToDelete);
            return RedirectToAction("Index");
        }
    }
}