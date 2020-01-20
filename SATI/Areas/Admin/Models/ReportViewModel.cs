using System.Collections.Generic;
using SATI.Entities;
using SATI.Models;

namespace SATI.Areas.Admin.Models
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            Reports = new List<Report>();
            Countries = new List<Country>();
            Regions = new List<Region>();
            Areas = new List<Area>();
            Languages = new List<Language>();

            Skills = new List<Skill>();
            Accreditations = new List<Accreditation>();
            Specialisations = new List<Specialisation>();
        }

        public List<Report> Reports { get; set; }

        #region Personal Details
        public List<Country> Countries { get; set; }
        public List<Region> Regions { get; set; }
        public List<Area> Areas { get; set; }
        public AvailabilityType? Availability { get; set; }
        #endregion

        #region Languages Lookup data
        public List<Language> Languages { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Accreditation> Accreditations { get; set; }
        public List<Specialisation> Specialisations { get; set; }
        #endregion

        #region Report Properties
        public List<KeyValuePair<string, bool>> FieldFilters { get; set; }
        public List<ReportItem> PersonalDetailsFilters { get; set; }
        public List<ReportItem> LanguageFilters { get; set; }
        #endregion
    }

    public class ReportDefinition
    {
        public ReportFieldFilters FieldFilters { get; set; }
        public ReportItem[] PersonalDetailsFilters { get; set; }
        public ReportItem[] LanguageFilters { get; set; }
    }

    public class ReportFieldFilters
    {
        public bool IsDeleted { get { return true; } }
        public bool MembershipNo { get; set; }
        public bool Title { get; set; }
        public bool FirstName { get; set; }
        public bool Initials { get; set; }
        public bool LastName { get; set; }
        public bool Email { get; set; }
        public bool MemberType { get; set; }
        public bool CellPhoneReport { get; set; }
        public bool AlternatePhoneReport { get; set; }
        public bool WorkPhoneReport { get; set; }
        public bool FaxReport { get; set; }
        public bool HoursPerWeekAvailable { get; set; }
        public bool CountryReport { get; set; }
        public bool Accredited { get; set; }
        public bool AvailabilityType { get; set; }
        public bool DateOfBirthReport { get; set; }
        public bool DateJoinedReport { get; set; }
        public bool MembershipYear { get; set; }
        public bool GroupsReport { get; set; }
        public bool LanguagesReport { get; set; }
        public bool CapabilitiesReport { get; set; }
        public bool Country { get; set; }
        public bool Region { get; set; }
        public bool Area { get; set; }
        public bool WorkAddressReport { get; set; }
        public bool StreetAddressReport { get; set; }
    }

    public class ReportItem
    {
        public string name { get; set; }
        public string field { get; set; }
        public string type { get; set; }
        public bool selected { get; set; }
        public ReportItemParam param { get; set; }
    }

    public class ReportItemParam
    {
        public string type { get; set; }
        public string value { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
    }
}