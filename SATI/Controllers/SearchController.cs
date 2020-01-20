using SATI.Entities;
using SATI.Models;
using SATI.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace SATI.Controllers
{
    public class SearchController : Controller
    {
        private SearchRepository repo = new SearchRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results(SearchViewModel model)
        {
            model.Users = repo.SearchUsers(model);
            return PartialView(model);
        }

        public ActionResult MemberDetails(string id)
        {
            var model = repo.GetUserDetails(id);
            return PartialView(model);
        }

        public ActionResult Search()
        {
            var model = new SearchViewModel
            {
                Languages = repo.All<Language>(),
                Skills = repo.All<Skill>().OrderBy(s => s.Name).ToList(),
                Specialisations = repo.All<Specialisation>(),
                Accreditations = repo.All<Accreditation>()
            };

            return PartialView(model);
        }
    }
}