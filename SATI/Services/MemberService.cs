using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SATI.Areas.Admin.Models;
using SATI.Entities;
using SATI.Models;
using SATI.Repositories;

namespace SATI.Services
{
    public class MemberService
    {
        public MembersRepository repo = new MembersRepository();

        public IEnumerable<object> SearchMembers(string searchTerm)
        {
            var users = repo.Where<User>(
                u => u.MembershipNo.ToString() == searchTerm ||
                     u.FirstName.Contains(searchTerm) ||
                     u.LastName.Contains(searchTerm) ||
                     u.Email.Contains(searchTerm))
                     .Select(u => new
                     {
                         u.Id,
                         u.FirstName,
                         u.LastName,
                         u.MembershipNo,
                         u.Email
                     })
                .ToList();

            return users;
        }

        internal User GetMember(object p)
        {
            throw new NotImplementedException();
        }

        public User GetMember(string id)
        {
            return repo.Where<User>(u => u.Id == id)
                .Include(u => u.WorkAddress)
                .Include(u => u.StreetAddress)
                .Include(u => u.UserSocialMediaLinks.Select(l => l.SocialMediaLink))
                .Include(u => u.Groups)
                .Include(u => u.Languages)
                .FirstOrDefault();
        }

        public List<User> GetAllMembers(string[] emailAddresses = null)
        {
            if (emailAddresses != null)
                return repo.Where<User>(u => !u.IsDeleted && emailAddresses.Contains(u.Email)).ToList();
            else
                return repo.Where<User>(u => !u.IsDeleted).ToList();
        } 

        public int GetNewMembershipNumber()
        {
            int membershipNo = repo._db.Users
                 .OrderByDescending(u => u.MembershipNo)
                 .FirstOrDefault()?.MembershipNo ?? 0;
            return membershipNo + 1;
        }

        public UserSaveResult SaveMember(MemberViewModel model, ApplicationUserManager userManager)
        {
            if (model.IsGeneralDetailsSave)
                return repo.SaveMemberGeneralDetails(model);
            else
                return repo.SaveMember(model, userManager);
        }

        public void DeleteUndeleteMember(string memberId)
        {
            var member = repo.Single<User>(u => u.Id == memberId);
            if (member.IsDeleted)
                repo.Undelete(member);
            else
                repo.SoftDelete(member);
        }

        public List<Area> GetAreas()
        {
            return repo._db.Areas
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToList();
        }

        public List<Region> GetRegions()
        {
            return repo._db.Regions
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .ToList();
        }

        public List<Country> GetCountries()
        {
            return repo.Where<Country>(c => !c.IsDeleted).OrderBy(c => c.Name).ToList();
        }

        public List<SelectListItem> GetMemberTitles()
        {
            return repo._db.Titles
                .Select(a => new SelectListItem
                {
                    Value = a.Name,
                    Text = a.Name
                })
                .ToList();
        }

        public List<SelectListItem> GetLanguages()
        {
            return repo._db.Languages
              .Select(a => new SelectListItem
              {
                  Value = a.LanguageId.ToString(),
                  Text = a.Name

              }).ToList();
        }

        public List<SocialMediaLink> GetSocialMediaLinks()
        {
            return repo.Where<SocialMediaLink>(s => !s.IsDeleted).ToList();
        }

        #region Groups

        public List<Group> GetGroups()
        {
            return repo.Where<Group>(g => !g.IsDeleted)
                .Include(g => g.Users)
                .OrderBy(g => g.Name)
                .ToList();
        }

        public void SaveGroup(Group group)
        {
            repo.Save(group);
        }

        public void DeleteGroup(int groupId)
        {
            var itemToDelete = repo.Single<Group>(g => g.GroupId == groupId);
            if (itemToDelete != null)
            {
                repo.SoftDelete(itemToDelete);
            }
            else
            {
                throw new Exception("Could not find Group to delete");
            }
        }
        #endregion

        #region Languages

        public List<Language> GetAllLanguages()
        {
            return repo.Where<Language>().ToList();
        }

        public void SaveMemberLanguage(string userId, int languageId)
        {
            repo.SaveMemberLanguage(userId, languageId);
        }

        public void DeleteMemberLanguage(string userId, int languageId)
        {
            repo.DeleteMemberLanguage(userId, languageId);
        }

        #endregion

        #region Capabilities

        public List<Skill> GetSkills()
        {
            return repo.Where<Skill>(s => !s.IsDeleted).OrderBy(s => s.Name).ToList();
        }

        public User GetMemberWithCapabilities(string memberId)
        {
            return repo.Where<User>(u => u.Id == memberId)
                .Include(u => u.Languages)
                .Include(u => u.Capabilities.Select(c => c.From))
                .Include(u => u.Capabilities.Select(c => c.To))
                .Include(u => u.Capabilities.Select(c => c.Skill))
                .Include(u => u.Capabilities.Select(c => c.Accreditations))
                .Include(u => u.Capabilities.Select(c => c.Specialisations))
                .FirstOrDefault();
        }

        public bool CheckSkillsCombinationIsValid(string memberId, int skillId, int fromLanguageId, int? toLanguageId)
        {
            var member = repo._db.Users
                .Include(u => u.Capabilities)
                .First(u => u.Id == memberId);

            if (!member.Capabilities.Any()) return true;

            // if the skill and language combinations exist then return false
            return !member.Capabilities.Any(c => c.SkillId == skillId
                                                && c.FromId == fromLanguageId
                                                && (toLanguageId == null || c.ToId == toLanguageId));
        }

        public void SaveCapability(string memberId, Capability capability)
        {
            repo.SaveCapability(memberId, capability);
        }

        public void DeleteCapability(int capabilityId)
        {
            repo.Delete(repo._db.Capabilities.Find(capabilityId));
        }

        public Capability GetCapability(int capabilityId)
        {
            return repo._db.Capabilities
                .Include(c => c.Skill)
                .Include(c => c.Accreditations)
                .Include(c => c.Specialisations)
                .First(c => c.CapabilityId == capabilityId);
        }

        public List<Accreditation> GetAllAccreditations()
        {
            return repo.Where<Accreditation>(a => !a.IsDeleted).ToList();
        }

        public List<Specialisation> GetAllSpecialisations()
        {
            return repo.Where<Specialisation>(a => !a.IsDeleted).ToList();
        }

        public void SaveAccreditations(int id, List<int> accrIds)
        {
            repo.SaveMemberAccreditations(id, accrIds);
        }

        public void SaveSpecialisations(int id, List<int> accrIds)
        {
            repo.SaveMemberSpecialisations(id, accrIds);
        }

        public void SaveIsAccredited(int id, bool isAccredited)
        {
            repo.SaveIsAccredited(id, isAccredited);
        }

        #endregion

        #region Payments
        public List<Payment> GetPayments(string memberId)
        {
            return repo.GetPayments(memberId);
        }

        public List<PaymentCategory> GetAllPaymentCategories()
        {
            return repo._db.PaymentCategories
                .Where(p => !p.IsDeleted)
                .ToList();
        }

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            return repo._db.PaymentMethods
                .Where(p => !p.IsDeleted)
                .ToList();
        }

        public void SavePayment(string memberId, Payment payment)
        {
            repo.SavePayment(memberId, payment);
        }

        #endregion
    }
}