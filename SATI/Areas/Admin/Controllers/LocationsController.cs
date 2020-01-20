using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Repositories;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LocationsController : Controller
    {
        private AdminRepository repo = new AdminRepository();

        public ActionResult Index(int? countryId, int? regionId)
        {
            var model = new LocationsViewModel
            {
                CountryId = countryId,
                RegionId = regionId,
                Countries = repo.Where<Country>(c=> !c.IsDeleted).ToList(),
                Regions = repo.Where<Region>(r => !r.IsDeleted).ToList(),
                Areas = repo.Where<Area>(a => !a.IsDeleted).ToList()
            };

            return View(model);
        }

        public ActionResult SaveCountry(int? countryId, string name)
        {
            Country country;
            if(countryId != null)
            {
                country = repo.Single<Country>(c => c.CountryId == countryId);
                country.Name = name;
                repo.Save(country);

            }
            else
            {
                country = new Country() { Name = name };
                repo.Save(country);
            }

            return Json(new { url = Url.Action("Index", new { countryId = country.CountryId}) });
        }

        public ActionResult SaveRegion(int countryId, int? regionId, string name)
        {
            Region region;
            if (regionId != null)
            {
                region = repo.Single<Region>(r => r.RegionId == regionId);
                region.Name = name;
                repo.Save(region);
            }
            else
            {
                region = new Region() { CountryId = countryId, Name = name };
                repo.Save(region);
            }

            return Json(new { url = Url.Action("Index", new { countryId = countryId, regionId = region.RegionId }) });
        }

        public ActionResult SaveArea(int countryId, int regionId, int? areaId, string name)
        {
            Area area;
            if (areaId != null)
            {
                area = repo.Single<Area>(r => r.AreaId == areaId);
                area.Name = name;
                repo.Save(area);
            }
            else
            {
                area = new Area() { RegionId = regionId, Name = name };
                repo.Save(area);
            }

            return Json( new { url = Url.Action("Index", new { countryId = countryId, regionId = regionId, areaId = area.AreaId }) });
        }

        public ActionResult Delete(int? selectedCountryId, int? selectedRegionId,
            int? countryToDeleteId, int? regionToDeleteId, int? areaToDeleteId)
        {
            if (countryToDeleteId != null)
                DeleteCountry(countryToDeleteId.Value);
            else if (regionToDeleteId != null)
                DeleteRegion(regionToDeleteId.Value);
            else if (areaToDeleteId != null)
                DeleteArea(areaToDeleteId.Value);

            return RedirectToAction("Index", new { countryId = selectedCountryId, regionId = selectedRegionId });
        }

        private void DeleteCountry(int countryToDeleteId)
        {
            var countryToDelete = repo.Where<Country>(c => c.CountryId == countryToDeleteId)
                    .Include(c => c.Regions.Select(r => r.Areas)).FirstOrDefault();

            if (countryToDelete != null)
            {
                countryToDelete.IsDeleted = true;
                repo.Save(countryToDelete);
                foreach (var r in countryToDelete.Regions)
                {
                    r.IsDeleted = true;
                    repo.Save(r);
                    foreach (var a in r.Areas)
                    {
                        a.IsDeleted = true;
                        repo.Save(a);
                    }
                }
            }
        }

        private void DeleteRegion(int regionToDeleteId)
        {
            var regionToDelete = repo.Where<Region>(r => r.RegionId == regionToDeleteId).Include(r => r.Areas).FirstOrDefault();
            regionToDelete.IsDeleted = true;
            repo.Save(regionToDelete);

            foreach (var a in regionToDelete.Areas)
            {
                a.IsDeleted = true;
                repo.Save(a);
            }
        }

        private void DeleteArea(int areaToDeleteId)
        {
            var areaToDelete = repo.Single<Area>(a => a.AreaId == areaToDeleteId);

            repo.SoftDelete(areaToDelete);
        }
    }
}