using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Repositories;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccreditationsController : Controller
    {
        private AdminRepository repo = new AdminRepository();
        public ActionResult Index()
        {
            var model = repo.Where<Accreditation>(a => !a.IsDeleted).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(Accreditation model)
        {
            repo.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var accreditationToDelete = repo.Single<Accreditation>(s => s.AccreditationId == id);
            repo.SoftDelete(accreditationToDelete);
            return RedirectToAction("Index");
        }
    }
}