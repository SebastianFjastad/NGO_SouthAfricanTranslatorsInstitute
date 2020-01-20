using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Models;
using Microsoft.AspNet.Identity;

namespace SATI.Repositories
{
    public class MembersRepository : BaseRepo
    {
        public UserSaveResult SaveMember(MemberViewModel model, ApplicationUserManager userManager)
        {
            var saveResult = new UserSaveResult();
            //add new user
            if (string.IsNullOrEmpty(model.User.Id))
            {
                model.User.Id = Guid.NewGuid().ToString();
                model.User.UserName = model.User.Email;

                int membershipNo = _db.Users
                    .OrderByDescending(u => u.MembershipNo)
                    .FirstOrDefault()?.MembershipNo ?? 0;

                model.User.ModifyPropertiesIfCompany(membershipNo + 1);

                model.User.LastUpdated = DateTime.Now;

                if (model.User.DateJoined == null)
                    model.User.DateJoined = DateTime.Now;

                IdentityResult result = userManager.Create(model.User, "p@$$w0rd");


                if (result.Succeeded)
                    userManager.AddToRole(model.User.Id, "Member");

                saveResult.Errors = string.Join("</br>", result.Errors);
                saveResult.User = model.User;
            }
            else
            {
                //update existing users
                var user = _db.Users.Where<User>(u => u.Id == model.User.Id)
                    .Include(u => u.WorkAddress)
                    .Include(u => u.StreetAddress)
                    .First();

                var _user = model.User;

                user.EmailConfirmed = true;
                user.TwoFactorEnabled = false;
                user.PhoneNumberConfirmed = true;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 0;

                user.CountryId = _user.CountryId;
                user.RegionId = _user.RegionId;
                user.AreaId = _user.AreaId;

                user.MemberType = _user.MemberType;
                user.Title = _user.Title;
                user.FirstName = _user.FirstName;
                user.Initials = _user.Initials;
                user.LastName = _user.LastName;
                user.PreferredName = _user.PreferredName;
                user.CompanyName = _user.CompanyName;

                user.WorkPhoneCode = _user.WorkPhoneCode;
                user.WorkPhone = _user.WorkPhone;
                user.AlternatePhoneCode = _user.AlternatePhoneCode;
                user.AlternatePhone = _user.AlternatePhone;
                user.CellPhoneCode = _user.CellPhoneCode;
                user.CellPhone = _user.CellPhone;
                user.FaxCode = _user.FaxCode;
                user.Fax = _user.Fax;
                user.Email = _user.Email;
                user.UserName = _user.Email;

                user.LastUpdated = DateTime.Now;

                _db.Entry(user.WorkAddress).CurrentValues.SetValues(_user.WorkAddress);
                _db.Entry(user.StreetAddress).CurrentValues.SetValues(_user.StreetAddress);

                _db.Entry(user.WorkAddress).State = _user.WorkAddress.AddressId == 0 ? EntityState.Added : EntityState.Modified;
                _db.Entry(user.StreetAddress).State = _user.StreetAddress.AddressId == 0 ? EntityState.Added : EntityState.Modified;

                try
                {
                    SaveChanges(_db);
                }
                catch (Exception ex)
                {
                    saveResult.Errors = ex.Message;
                }
                saveResult.User = user;
            }
            return saveResult;
        }

        public UserSaveResult SaveMemberGeneralDetails(MemberViewModel model)
        {

            var saveResult = new UserSaveResult();

            var user = _db.Users
                .Include(u => u.Groups)
                .Include(u => u.UserSocialMediaLinks)
                .Include(u => u.WorkAddress)
                .Include(u => u.StreetAddress)
                .First(u => u.Id == model.User.Id);

            RemoveUsersGroups(model, user);

            AddUsersGroups(model, user);

            AddSocialMediaLinks(model, user);

            var _user = model.User;
            user.GeneralNotes = _user.GeneralNotes;
            user.Qualifications = _user.Qualifications;
            user.FirstLanguageId = _user.FirstLanguageId;
            user.Website = _user.Website;
            user.HoursPerWeekAvailable = _user.HoursPerWeekAvailable;
            user.WorksAtAgency = _user.WorksAtAgency;
            user.AvailabilityType = _user.AvailabilityType;
            user.MembershipYear = _user.MembershipYear;
            user.DateOfBirth = _user.DateOfBirth;
            user.DateJoined = _user.DateJoined;
            user.LastUpdated = DateTime.Now;

            try
            {
                SaveChanges(_db);
            }
            catch (Exception ex)
            {
                saveResult.Errors = ex.Message;
            }

            saveResult.User = user;

            return saveResult;
        }

        private void AddUsersGroups(MemberViewModel model, User user)
        {
            var groups = model.OtherGroups
                .Where(g => g.IsSelected)
                .Select(x => new Group { GroupId = x.Id })
                .ToList();

            groups.ForEach(g => _db.Groups.Attach(g));

            groups.ForEach(g => user.Groups.Add(g));
        }

        private void RemoveUsersGroups(MemberViewModel model, User user)
        {
            var groups = model.UsersGroups
                .Where(g => !g.IsSelected)
                .Select(x => new Group { GroupId = x.Id })
                .ToList();

            groups.ForEach(g => user.Groups.Remove(user.Groups.First(ug => ug.GroupId == g.GroupId)));
        }

        private void AddSocialMediaLinks(MemberViewModel model, User user)
        {
            user.UserSocialMediaLinks.Clear();

            var itemsToAdd = model.SocialMediaLinkItems
                .Where(l => !string.IsNullOrEmpty(l.Link) && !string.IsNullOrWhiteSpace(l.Link)).ToList();

            var items = itemsToAdd.Select(i =>
                    new UserSocialMediaLink
                    {
                        SocialMediaLinkId = i.SocialMediaLinkId,
                        Link = i.Link,
                        UserId = user.Id
                    }).ToList();

            user.UserSocialMediaLinks.AddRange(items);
        }

        public void SaveMemberLanguage(string userId, int languageId)
        {
            var user = _db.Users.Find(userId);

            var lang = _db.Languages.Find(languageId);

            user.Languages.Add(lang);

            _db.SaveChanges();
        }

        public void DeleteMemberLanguage(string userId, int langaugeId)
        {
            var user = _db.Users.Include(u => u.Languages).Single(u => u.Id == userId);

            user.Languages.Remove(user.Languages.First(l => l.LanguageId == langaugeId));

            _db.SaveChanges();
        }

        public void SaveCapability(string memberId, Capability capability)
        {
            var member = _db.Users.Find(memberId);

            member.Capabilities.Add(capability);

            _db.SaveChanges();

        }

        public void SaveMemberAccreditations(int capabilityId, List<int> ids)
        {
            if (ids == null)
                ids = new List<int>();

            var capability = _db.Capabilities
                    .Include(a => a.Accreditations)
                    .First(c => c.CapabilityId == capabilityId);

            capability.Accreditations.Clear();

            var accrToAdd = _db.Accreditations.Where(a => ids.Contains(a.AccreditationId)).ToList();

            capability.Accreditations.AddRange(accrToAdd);

            _db.SaveChanges();

        }

        public void SaveMemberSpecialisations(int capabilityId, List<int> ids)
        {
            if (ids.Any())
            {
                var specToAdd = _db.Specialisations.Where(s => ids.Contains(s.SpecialisationId)).ToList();

                var capability = _db.Capabilities
                    .Include(a => a.Specialisations)
                    .First(c => c.CapabilityId == capabilityId);

                capability.Specialisations.Clear();

                capability.Specialisations.AddRange(specToAdd);

                _db.SaveChanges();
            }
        }

        public void SaveIsAccredited(int capabilityId, bool isAccredited)
        {
            var capability = _db.Capabilities.Find(capabilityId);
            if (capability == null) return;
            capability.IsAccredited = isAccredited;
            _db.SaveChanges();
        }

        public List<Payment> GetPayments(string memberId)
        {
            var member = _db.Users
                .Include(p => p.Payments)
                .Include(p => p.Payments.Select(x => x.PaymentCategory))
                .Include(p => p.Payments.Select(x => x.PaymentMethod))
                .FirstOrDefault(u => u.Id == memberId);

            return member.Payments.ToList();
        }

        public void SavePayment(string memberId, Payment payment)
        {
            var member = _db.Users
                .Include(u => u.Payments)
                .First(u => u.Id == memberId);

            member.Payments.Add(payment);

            _db.SaveChanges();
        }
    }

    public class UserSaveResult
    {
        public string Errors { get; set; }
        public bool Success { get { return string.IsNullOrEmpty(Errors); } }
        public User User { get; set; }
    }
}