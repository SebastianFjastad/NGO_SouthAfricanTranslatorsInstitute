using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Repositories;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialisationsController : Controller
    {
        private AdminRepository repo = new AdminRepository();
        public ActionResult Index()
        {
            var model = repo.Where<Specialisation>(a => !a.IsDeleted).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(Specialisation model)
        {
            repo.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var specialisationToDelete = repo.Single<Specialisation>(s => s.SpecialisationId == id);
            repo.SoftDelete(specialisationToDelete);
            return RedirectToAction("Index");
        }
    }
}