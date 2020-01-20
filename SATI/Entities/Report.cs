
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SATI.Entities
{
    public class Report
    {
        public Report(int reportId, string reportName, string reportJson, DateTime lastRunDate)
        {
            ReportId = reportId;
            ReportName = reportName;
            ReportJson = reportJson;
            LastRunDate = lastRunDate;
        }

        protected Report()
        {

        }

        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportJson { get; set; }
        public DateTime? LastRunDate { get; set; }
        [NotMapped]
        public string LastRunDateString { get { return LastRunDate?.ToString("dd MMM yy HH:mm") ?? ""; } }
    }
}