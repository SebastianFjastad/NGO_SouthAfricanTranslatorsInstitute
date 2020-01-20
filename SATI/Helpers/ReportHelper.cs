using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OfficeOpenXml;
using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Models;
using SATI.Repositories;
using WebGrease.Css.Extensions;
using SATI.Context;

namespace SATI.Helpers
{
    public static class ReportHelper
    {
        public static ExcelPackage GetExcelExport(Report report)
        {
            ReportDefinition reportDefinition = JsonConvert.DeserializeObject<ReportDefinition>(report.ReportJson);

            ExcelPackage pkg = new ExcelPackage();
            ExcelWorksheet ws;

            pkg.Workbook.Properties.Author = "SATI";
            pkg.Workbook.Properties.Title = report.ReportName;
            pkg.Workbook.Worksheets.Add("Members");
            ws = pkg.Workbook.Worksheets[1];
            var data = GetReportData(reportDefinition);
            WriteReportData(ws, data, reportDefinition);

            return pkg;
        }

        private static void WriteReportData(ExcelWorksheet ws, IEnumerable<User> data, ReportDefinition report)
        {
            int row = 1, col = 1; //the Excel offsets are 1-based
            ArrayList fieldNames = new ArrayList(); //always include the is deleted flag because the report includes deleted (inactive) members.
            //set column headings based on selected output fields
            User dummyUser = new User(); //to get column name attributes
            report.FieldFilters.GetType()
                .GetProperties()
                .Where(pi =>
                    pi.PropertyType == typeof(bool) && pi.GetGetMethod() != null &&
                    (bool)pi.GetGetMethod().Invoke(report.FieldFilters, null))
                .ForEach(pi =>
                {
                    fieldNames.Add(pi.Name);
                    //default to db field name
                    string fieldName = pi.Name;
                    PropertyInfo dummyPi = dummyUser.GetType().GetProperty(pi.Name);
                    if (dummyPi != null)
                    {
                        //look for [DisplayName("Fancy name")]
                        var dispNameAttr = (DisplayNameAttribute)dummyPi.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault();
                        if (dispNameAttr != null)
                            fieldName = dispNameAttr.DisplayName;
                        else
                        {
                            //now look for [Display(Name="Fancy name")]
                            var dispAttr = (DisplayAttribute)dummyPi.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                            if (dispAttr != null)
                                fieldName = dispAttr.Name;
                        }
                    }
                    ws.Cells[row, col++].Value = fieldName;
                });
            //output data
            foreach (User user in data)
            {
                row++;
                col = 1;
                var userType = user.GetType();
                foreach (string fieldName in fieldNames)
                {
                    PropertyInfo fieldProperty = userType.GetProperty(fieldName);
                    if (fieldProperty != null)
                        ws.Cells[row, col].Value = fieldProperty.GetValue(user, null);
                    else
                        ws.Cells[row, col].Value = fieldName + " property not found on User";
                    col++;
                }
            }
        }


        private static IEnumerable<User> GetReportData(ReportDefinition report)
        {
            AdminRepository repo = new AdminRepository();

            var context = Db.GetReadOnlyInstance();

            context.Countries.Load();
            context.Regions.Load();
            context.Languages.Load();
            context.Groups.Load();

            #region old report filters
            //IQueryable<User> data = context.Users
            //    //.Where(u => !u.IsDeleted)
            //    .Include(u => u.WorkAddress)
            //    .Include(u => u.StreetAddress)
            //    .Include(u => u.Capabilities.Select(c => c.From))
            //    .Include(u => u.Capabilities.Select(c => c.To))
            //    .Include(u => u.Capabilities.Select(c => c.Skill));


            //#region Personal Details Filters
            //int intParam;
            //DateTime dateParam, date2Param;
            //foreach (ReportItem r in report.PersonalDetailsFilters.Where(r => r.selected))
            //{
            //    switch (r.field)
            //    {
            //        case "FirstName":
            //            if (!string.IsNullOrEmpty(r.param.value))
            //                switch (r.param.type)
            //                {
            //                    case "exact":
            //                        data = data.Where(u => u.FirstName == r.param.value);
            //                        break;
            //                    case "start":
            //                        data = data.Where(u => u.FirstName.StartsWith(r.param.value));
            //                        break;
            //                    case "contains":
            //                        data = data.Where(u => u.FirstName.Contains(r.param.value));
            //                        break;
            //                }
            //            break;
            //        case "LastName":
            //            if (!string.IsNullOrEmpty(r.param.value))
            //                switch (r.param.type)
            //                {
            //                    case "exact":
            //                        data = data.Where(u => u.LastName == r.param.value);
            //                        break;
            //                    case "start":
            //                        data = data.Where(u => u.LastName.StartsWith(r.param.value));
            //                        break;
            //                    case "contains":
            //                        data = data.Where(u => u.LastName.Contains(r.param.value));
            //                        break;
            //                }
            //            break;
            //        case "MemberType":
            //            switch (r.param.value)
            //            {
            //                case "true":
            //                    data = data.Where(u => u.MemberType == MemberType.Individual);
            //                    break;
            //                case "false":
            //                    data = data.Where(u => u.MemberType == MemberType.Company);
            //                    break;
            //                case "ignore":
            //                    break;
            //            }
            //            break;
            //        case "CountryId":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.CountryId == intParam);
            //            break;
            //        case "RegionId":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.RegionId == intParam);
            //            break;
            //        case "AreaId":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.AreaId == intParam);
            //            break;
            //        case "FirstLanguageId":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.FirstLanguageId == intParam);
            //            break;
            //        case "GroupId":
            //            if (!string.IsNullOrEmpty(r.param.value))
            //                switch (r.param.type)
            //                {
            //                    case "exact":
            //                        data = data.Where(u => u.Groups.Any(g => g.Name == r.param.value));
            //                        break;
            //                    case "start":
            //                        data = data.Where(u => u.Groups.Any(g => g.Name.StartsWith(r.param.value)));
            //                        break;
            //                    case "contains":
            //                        data = data.Where(u => u.Groups.Any(g => g.Name.Contains(r.param.value)));
            //                        break;
            //                }
            //            break;
            //        case "Employer":
            //            switch (r.param.type)
            //            {
            //                case "exact":
            //                    data = data.Where(u => u.CompanyName == r.param.value);
            //                    break;
            //                case "start":
            //                    data = data.Where(u => u.CompanyName.StartsWith(r.param.value));
            //                    break;
            //                case "contains":
            //                    data = data.Where(u => u.CompanyName.Contains(r.param.value));
            //                    break;
            //            }
            //            break;
            //        case "IsFreelance":
            //            switch (r.param.value)
            //            {
            //                case "true":
            //                    data = data.Where(u => u.MemberType == MemberType.Individual && string.IsNullOrEmpty(u.CompanyName));
            //                    break;
            //                case "false":
            //                    data = data.Where(u => u.MemberType == MemberType.Individual && !string.IsNullOrEmpty(u.CompanyName));
            //                    break;
            //                case "ignore":
            //                    break;
            //            }
            //            break;

            //        case "Availability":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.AvailabilityType == (AvailabilityType)intParam);
            //            break;
            //        case "IsAgency":
            //            switch (r.param.value)
            //            {
            //                case "true":
            //                    data = data.Where(u => u.MemberType == MemberType.Company);
            //                    break;
            //                case "false":
            //                    data = data.Where(u => u.MemberType != MemberType.Company);
            //                    break;
            //                case "ignore":
            //                    break;
            //            }
            //            break;
            //        case "MembershipYear":
            //            if (DateTime.TryParseExact(r.param.value1, "yyyy/MM/dd", CultureInfo.InvariantCulture,
            //                DateTimeStyles.AssumeLocal, out dateParam))
            //                data = data.Where(u => u.MembershipYear >= dateParam.Year);
            //            if (DateTime.TryParseExact(r.param.value2, "yyyy/MM/dd", CultureInfo.InvariantCulture,
            //                DateTimeStyles.AssumeLocal, out date2Param))
            //                data = data.Where(u => u.MembershipYear <= date2Param.Year);
            //            break;
            //        case "LastModified":
            //            if (DateTime.TryParseExact(r.param.value1, "yyyy/MM/dd", CultureInfo.InvariantCulture,
            //                DateTimeStyles.AssumeLocal, out dateParam))
            //                data = data.Where(u => u.LastUpdated >= dateParam);
            //            if (DateTime.TryParseExact(r.param.value2, "yyyy/MM/dd", CultureInfo.InvariantCulture,
            //                DateTimeStyles.AssumeLocal, out date2Param))
            //            {
            //                //search until 1 second before midnight of the end date
            //                //so as to include records modified DURING that date
            //                DateTime midnightOfEndDate = date2Param.AddDays(1).AddSeconds(-1);
            //                data = data.Where(u => u.LastUpdated <= midnightOfEndDate);
            //            }
            //            break;
            //    }
            //}
            //#endregion


            //#region language filters
            //foreach (ReportItem r in report.LanguageFilters.Where(r => r.selected))
            //{
            //    switch (r.field)
            //    {
            //        case "FromLanguage":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.Capabilities.Any(c => c.FromId == intParam));
            //            break;
            //        case "ToLanguage":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.Capabilities.Any(c => c.ToId == intParam));
            //            break;
            //        case "Skill":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.Capabilities.Any(c => c.SkillId == intParam));
            //            break;
            //        case "Accreditation":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.Capabilities.Any(c => c.Accreditations.Any(a => a.AccreditationId == intParam)));
            //            break;
            //        case "Specialisation":
            //            if (int.TryParse(r.param.value, out intParam))
            //                data = data.Where(u => u.Capabilities.Any(c => c.Specialisations.Any(a => a.SpecialisationId == intParam)));
            //            break;
            //        case "IsAccredited":
            //            bool isAccredited;
            //            if (bool.TryParse(r.param.value, out isAccredited))
            //                data = data.Where(u => u.Capabilities.Any(c => c.IsAccredited == isAccredited));
            //            break;
            //    }
            //}
            //#endregion
            #endregion


            IEnumerable<User> x = context.Users
                //.Where(u => !u.IsDeleted)
                .Include(u => u.WorkAddress)
                .Include(u => u.StreetAddress)
                .Include(u => u.Capabilities.Select(c => c.From))
                .Include(u => u.Capabilities.Select(c => c.To))
                .Include(u => u.Capabilities.Select(c => c.Skill));

            IEnumerable<User> data = x.ToList();

            #region Personal Details Filters
            int intParam;
            DateTime dateParam, date2Param;
            foreach (ReportItem r in report.PersonalDetailsFilters.Where(r => r.selected))
            {
                switch (r.field)
                {
                    case "FirstName":
                        if (!string.IsNullOrEmpty(r.param.value))
                            switch (r.param.type)
                            {
                                case "exact":
                                    data = data.Where(u => u.FirstName == r.param.value);
                                    break;
                                case "start":
                                    data = data.Where(u => u.FirstName.StartsWith(r.param.value));
                                    break;
                                case "contains":
                                    data = data.Where(u => u.FirstName.Contains(r.param.value));
                                    break;
                            }
                        break;
                    case "LastName":
                        if (!string.IsNullOrEmpty(r.param.value))
                            switch (r.param.type)
                            {
                                case "exact":
                                    data = data.Where(u => u.LastName == r.param.value);
                                    break;
                                case "start":
                                    data = data.Where(u => u.LastName.StartsWith(r.param.value));
                                    break;
                                case "contains":
                                    data = data.Where(u => u.LastName.Contains(r.param.value));
                                    break;
                            }
                        break;
                    case "MemberType":
                        switch (r.param.value)
                        {
                            case "true":
                                data = data.Where(u => u.MemberType == MemberType.Individual);
                                break;
                            case "false":
                                data = data.Where(u => u.MemberType == MemberType.Company);
                                break;
                            case "ignore":
                                break;
                        }
                        break;
                    case "CountryId":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.CountryId == intParam).ToList();
                        break;
                    case "RegionId":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.RegionId == intParam).ToList();
                        break;
                    case "AreaId":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.AreaId == intParam).ToList();
                        break;
                    case "FirstLanguageId":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.FirstLanguageId == intParam);
                        break;
                    case "GroupId":
                        if (!string.IsNullOrEmpty(r.param.value))
                            switch (r.param.type)
                            {
                                case "exact":
                                    data = data.Where(u => u.Groups.Any(g => g.Name == r.param.value));
                                    break;
                                case "start":
                                    data = data.Where(u => u.Groups.Any(g => g.Name.StartsWith(r.param.value)));
                                    break;
                                case "contains":
                                    data = data.Where(u => u.Groups.Any(g => g.Name.Contains(r.param.value)));
                                    break;
                            }
                        break;
                    case "Employer":
                        switch (r.param.type)
                        {
                            case "exact":
                                data = data.Where(u => u.CompanyName == r.param.value);
                                break;
                            case "start":
                                data = data.Where(u => u.CompanyName.StartsWith(r.param.value));
                                break;
                            case "contains":
                                data = data.Where(u => u.CompanyName.Contains(r.param.value));
                                break;
                        }
                        break;
                    case "IsFreelance":
                        switch (r.param.value)
                        {
                            case "true":
                                data = data.Where(u => u.MemberType == MemberType.Individual && string.IsNullOrEmpty(u.CompanyName));
                                break;
                            case "false":
                                data = data.Where(u => u.MemberType == MemberType.Individual && !string.IsNullOrEmpty(u.CompanyName));
                                break;
                            case "ignore":
                                break;
                        }
                        break;

                    case "Availability":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.AvailabilityType == (AvailabilityType)intParam);
                        break;
                    case "IsAgency":
                        switch (r.param.value)
                        {
                            case "true":
                                data = data.Where(u => u.MemberType == MemberType.Company);
                                break;
                            case "false":
                                data = data.Where(u => u.MemberType != MemberType.Company);
                                break;
                            case "ignore":
                                break;
                        }
                        break;
                    case "MembershipYear":
                        if (DateTime.TryParseExact(r.param.value1, "yyyy/MM/dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out dateParam))
                            data = data.Where(u => u.MembershipYear >= dateParam.Year);
                        if (DateTime.TryParseExact(r.param.value2, "yyyy/MM/dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out date2Param))
                            data = data.Where(u => u.MembershipYear <= date2Param.Year);
                        break;
                    case "LastModified":
                        if (DateTime.TryParseExact(r.param.value1, "yyyy/MM/dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out dateParam))
                            data = data.Where(u => u.LastUpdated >= dateParam);
                        if (DateTime.TryParseExact(r.param.value2, "yyyy/MM/dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out date2Param))
                        {
                            //search until 1 second before midnight of the end date
                            //so as to include records modified DURING that date
                            DateTime midnightOfEndDate = date2Param.AddDays(1).AddSeconds(-1);
                            data = data.Where(u => u.LastUpdated <= midnightOfEndDate);
                        }
                        break;
                }
            }
            #endregion


            var from = false;
            var to = false;
            var skill = false;


            var fromItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "FromLanguage");
            var toItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "ToLanguage");
            var skillItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "Skill");

            var accrItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "Accreditation");
            var specItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "Specialisation");
            var isAccrItem = report.LanguageFilters.FirstOrDefault(r => r.selected && r.field == "IsAccredited");

            var fromId = fromItem != null ? int.Parse(fromItem.param.value) : 0;
            var toId = toItem != null ? int.Parse(toItem.param.value) : 0;
            var skillId = skillItem != null ? int.Parse(skillItem.param.value) : 0;

            var accrId = accrItem != null ? int.Parse(accrItem.param.value) : 0;
            var specId = accrItem != null ? int.Parse(specItem.param.value) : 0;

            bool? isAccr = null;

            if (isAccrItem != null)
                isAccr = bool.Parse(accrItem.field);

            if ((fromId + toId + skillId) > 0)
            {
                data = data.Where(d => d.Capabilities
                .Any(c =>
                    (fromId == 0 || c.FromId == fromId) &&
                    (toId == 0 || c.ToId == toId) &&
                    (skillId == 0 || c.SkillId == skillId) &&
                    (accrId == 0 || c.Accreditations.Any(ac => ac.AccreditationId == accrId)) &&
                    (specId == 0 || c.Specialisations.Any(a => a.SpecialisationId == specId)) &&
                    (isAccr == null || c.IsAccredited == isAccr)

                )).ToList();
            }

            #region language filters
            foreach (ReportItem r in report.LanguageFilters.Where(r => r.selected))
            {
                switch (r.field)
                {
                    case "Accreditation":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.Capabilities.Any(c => c.Accreditations.Any(a => a.AccreditationId == intParam))).ToList();
                        break;
                    case "Specialisation":
                        if (int.TryParse(r.param.value, out intParam))
                            data = data.Where(u => u.Capabilities.Any(c => c.Specialisations.Any(a => a.SpecialisationId == intParam))).ToList();
                        break;
                    case "IsAccredited":
                        bool isAccredited;
                        if (bool.TryParse(r.param.value, out isAccredited))
                            data = data.Where(u => u.Capabilities.Any(c => c.IsAccredited == isAccredited)).ToList();
                        break;
                }
            }
            #endregion

            var results = data.OrderBy(r => Guid.NewGuid()).ToList();
            return results;
        }

    }
}