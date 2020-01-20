using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SATI.Entities;
using SATI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SATI.Areas.Admin.Models
{
    public class MemberViewModel
    {
        public MemberViewModel()
        {
            User = new User { Id = null };
            Titles = new List<SelectListItem>();
            Groups = new List<Group>();
            UsersGroups = new List<CheckboxItemViewModel>();
            OtherGroups = new List<CheckboxItemViewModel>();
            SocialMediaLinkItems = new List<SocialMediaLinkViewModel>();
            SocialMediaLinks = new List<SocialMediaLink>();
            Languages = new List<SelectListItem>();
            Countries = new List<Country>();
            Regions = new List<Region>();
            Areas = new List<Area>();
            MaxMembershipYear = DateTime.Now.Year + 1;
        }

        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public List<Area> Areas { get; set; }
        public List<Region> Regions { get; set; }
        public List<SelectListItem> Languages { get; set; }
        public List<CheckboxItemViewModel> OtherGroups { get; set; }
        public List<CheckboxItemViewModel> UsersGroups { get; set; }
        public List<SocialMediaLinkViewModel> SocialMediaLinkItems { get; set; }
        public List<SocialMediaLink> SocialMediaLinks { get; set; }
        public User User { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool IsGeneralDetailsSave { get; set; }
        public int MaxMembershipYear { get; }
        public int? MinMembershipYear
        {
            get
            {
                if (User?.MembershipYear != null)
                {
                    return User.MembershipYear;
                }
                return DateTime.Now.Year - 1;
            }
        }
        public void SortData()
        {
            if (SocialMediaLinks.Any() && User != null)
            {
                //links which the user does not already have
                var nonUserSocialMediaLinks = SocialMediaLinks
                    .Where(s => User.UserSocialMediaLinks.All(l => l.SocialMediaLinkId != s.SocialMediaLinkId)).ToList();

                var mappedNonUserLinks = nonUserSocialMediaLinks
                    .Select(x => new SocialMediaLinkViewModel
                    {
                        SocialMediaLinkId = x.SocialMediaLinkId,
                        Name = x.Name
                    }).ToList();

                //links which the user already has
                var userSocialMediaLinks = SocialMediaLinks
                    .Where(s => User.UserSocialMediaLinks.Any(l => l.SocialMediaLinkId == s.SocialMediaLinkId)).ToList();

                var mappedUserLinks = userSocialMediaLinks.Join(User.UserSocialMediaLinks,
                        ul => ul.SocialMediaLinkId,
                        l => l.SocialMediaLinkId,
                        (ul, l) => new SocialMediaLinkViewModel
                        {
                            SocialMediaLinkId = ul.SocialMediaLinkId,
                            Name = ul.Name,
                            Link = l.Link
                        }
                    ).ToList();

                SocialMediaLinkItems.AddRange(mappedUserLinks);

                SocialMediaLinkItems.AddRange(mappedNonUserLinks);
            }

            if (User != null && User.Groups.Any())
            {
                UsersGroups = User.Groups.Select(
                    g => new CheckboxItemViewModel
                    {
                        Id = g.GroupId,
                        Name = g.Name,
                        IsSelected = true
                    }).ToList();
            }

            OtherGroups = Groups.Where(g => User.Groups.All(ug => ug.GroupId != g.GroupId))
                    .Select(g => new CheckboxItemViewModel
                    {
                        Id = g.GroupId,
                        Name = g.Name,
                        IsSelected = false
                    }).ToList();
        }
    }

    public class CheckboxItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SocialMediaLinkViewModel
    {
        public int SocialMediaLinkId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}