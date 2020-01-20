using System.Collections.Generic;
using System.Data.Entity;
using SATI.Models;
using System.Linq;
using System;

namespace SATI.Repositories
{
    public class SearchRepository : BaseRepo
    {
        public List<User> SearchUsers(SearchViewModel model)
        {
            model.SearchTerm = model.SearchTerm?.ToLower();

            IQueryable<User> query = Where<User>(
                u => !u.IsDeleted &&
                (model.SearchTerm == null 
                || u.FirstName.ToLower().Contains(model.SearchTerm)
                || u.LastName.ToLower().Contains(model.SearchTerm)
                || u.Email.ToLower().Contains(model.SearchTerm)))
                .Include(u => u.Area)
                .Include(u => u.Region)
                .AsNoTracking();


            var fromId = model.FromLanguageId ?? 0;
            var toId = model.ToLanguageId ?? 0;
            var skillId = model.SkillId ?? 0;
            var accrId = model.AccreditationId ?? 0;
            var specId = model.SpecialisationId ?? 0;

            if((fromId + toId + skillId) > 0)
            {
                query = query.Include(u => u.Capabilities.Select(c => c.From))
                    .Include(u => u.Capabilities.Select(c => c.To))
                    .Include(u => u.Capabilities.Select(c => c.Skill));

                if ((accrId + specId) > 0)
                    query = query
                        .Include(u => u.Capabilities.Select(c => c.Accreditations))
                        .Include(u => u.Capabilities.Select(c => c.Specialisations));

                query = query.Where(u => u.Capabilities
                    .Any(c => 
                        (fromId == 0 || fromId == c.FromId) 
                        && (toId == 0 || toId == c.ToId) &&
                        (skillId == 0 || skillId == c.SkillId) &&
                        (accrId == 0 || c.Accreditations.Any(ac => ac.AccreditationId == accrId)) &&
                        (specId == 0 || c.Specialisations.Any(a => a.SpecialisationId == specId))
                    ));
            }

            var results = query.ToList();

            results.OrderBy(u => Guid.NewGuid()).ToList();

            return query.ToList().OrderBy(u => Guid.NewGuid()).ToList();
        }

        public User GetUserDetails(string id)
        {
            _db.Countries.Load();
            _db.Groups.Load();
            _db.Areas.Load();
            _db.Regions.Load();

            var result = _db.Users
                .Include(u => u.StreetAddress)
                .Include(u => u.WorkAddress)
                .Include(u => u.UserSocialMediaLinks.Select(us => us.SocialMediaLink))
                .Include(u => u.Capabilities.Select(c => c.From))
                .Include(u => u.Capabilities.Select(c => c.To))
                .Include(u => u.Capabilities.Select(c => c.Skill))
                .Include(u => u.Capabilities.Select(c => c.Specialisations))
                .Include(u => u.Capabilities.Select(c => c.Accreditations))
                .FirstOrDefault();

            return result;
        }
    }
}