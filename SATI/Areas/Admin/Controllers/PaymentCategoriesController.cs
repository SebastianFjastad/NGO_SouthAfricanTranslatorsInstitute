using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Repositories;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentCategoriesController : Controller
    {
        private AdminRepository repo = new AdminRepository();

        public ActionResult Index()
        {
            var model = repo.Where<PaymentCategory>(s => !s.IsDeleted).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(PaymentCategory model)
        {
            repo.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var paymentCategoryToDelete = repo.Single<PaymentCategory>(p => p.PaymentCategoryId == id);
            repo.SoftDelete(paymentCategoryToDelete);
            return RedirectToAction("Index");
        }
    }
}