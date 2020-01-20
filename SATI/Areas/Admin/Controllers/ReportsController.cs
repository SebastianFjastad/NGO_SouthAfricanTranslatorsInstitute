using System.IO;
using SATI.Areas.Admin.Models;
using SATI.Repositories;
using System.Web.Mvc;
using SATI.Entities;
using System.Linq;
using OfficeOpenXml;
using SATI.Helpers;
using System;

namespace SATI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private AdminRepository repo = new AdminRepository();

        public ActionResult Index()
        {
            var model = new ReportViewModel
            {
                Reports = repo.All<Report>().OrderByDescending(r => r.LastRunDate).ToList(),
                Countries = repo.Where<Country>(c => !c.IsDeleted).ToList(),
                Regions = repo.Where<Region>(r => !r.IsDeleted).ToList(),
                Areas = repo.Where<Area>(a => !a.IsDeleted).ToList(),
                Languages = repo.All<Language>().ToList(),
                Accreditations = repo.All<Accreditation>().ToList(),
                Skills = repo.All<Skill>().OrderBy(s => s.Name).ToList(),
                Specialisations = repo.All<Specialisation>().ToList()
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveReport(ReportObject model)
        {
            var reportId = model.reportId;
            Report report = null;
            if (model.reportId != 0)
                report = repo.Single<Report>(r => r.ReportId == model.reportId);

            if (report != null)
            {
                //compare the existing report json and name to the one to save to determine if a new report should be created
                if (report.ReportName != model.reportName || report.ReportJson != model.reportJson)
                {
                    var savedReport = repo.Save(new Report(0, model.reportName, model.reportJson, DateTime.Now));
                    reportId = savedReport.ReportId;
                }
                else
                {
                    //if there are no changes to the report then save it
                    report.LastRunDate = DateTime.Now;
                    var savedReport = repo.Save(report);
                    reportId = savedReport.ReportId;
                }
            }
            else
            {
                //create new report
                var savedReport = repo.Save(new Report(0, model.reportName, model.reportJson, DateTime.Now));
                reportId = savedReport.ReportId;
            }

            return Json(new
            {
                url = Url.Action("RunReport", new { id = reportId })
            },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadReport(int reportId)
        {
            var report = repo.Single<Report>(r => r.ReportId == reportId);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public FileResult RunReport(int id)
        {
            var report = repo.Single<Report>(r => r.ReportId == id);

            string fileDownloadName = "SATI " + report.ReportName + ".xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            ExcelPackage package = ReportHelper.GetExcelExport(report);

            var memStream = new MemoryStream();
            package.SaveAs(memStream);
            memStream.Position = 0;

            return new FileStreamResult(memStream, contentType) { FileDownloadName = fileDownloadName };
        }

        public JsonResult DeleteReport(int reportId)
        {
            var reportToDelete = repo.Single<Report>(r => r.ReportId == reportId);
            repo.Delete(reportToDelete);
            return Json(new { deleted = true }, JsonRequestBehavior.AllowGet);
        }
    }

    public class ReportObject
    {
        public int reportId { get; set; }
        public string reportJson { get; set; }
        public string reportName { get; set; }
    }
}