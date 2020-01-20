using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Repositories;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SocialMediaController : Controller
    {
        private AdminRepository repo = new AdminRepository();

        public ActionResult Index()
        {
            var model = repo.Where<SocialMediaLink>(x => !x.IsDeleted).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(SocialMediaLink model)
        {
            repo.Save(model);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var itemToDelete = repo.Single<SocialMediaLink>(x => x.SocialMediaLinkId == id);

            repo.SoftDelete(itemToDelete);

            return RedirectToAction("Index");
        }
    }
}