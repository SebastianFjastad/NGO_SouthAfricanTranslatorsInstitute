using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SATI.Entities;

namespace SATI.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            WorkAddress = new Address();
            StreetAddress = new Address();
            Groups = new List<Group>();
            UserSocialMediaLinks = new List<UserSocialMediaLink>();
            Capabilities = new List<Capability>();
            Languages = new List<Language>();
            Payments = new List<Payment>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public int MembershipNo { get; set; }


        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public override string Email { get; set; }

        public override string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Preferred Name")]
        public string PreferredName { get; set; }
        public string Title { get; set; }
        public string Initials { get; set; }

        [Display(Name = "Alternate Email")]
        public string AlternateEmail { get; set; }
        public string WorkPhoneCode { get; set; }

        [Display(Name = "Country")]
        [NotMapped]
        public string CountryReport => Country.Name;
        [DisplayName("Home phone")]
        [NotMapped]
        public string AlternatePhoneReport => AlternatePhoneCode + AlternatePhone;
        [Display(Name = "Cell phone")]
        [NotMapped]
        public string CellPhoneReport => CellPhoneCode + CellPhone;
        [Display(Name = "Work phone")]
        [NotMapped]
        public string WorkPhoneReport => WorkPhoneCode + WorkPhone;
        [Display(Name = "Fax")]
        [NotMapped]
        public string FaxReport => FaxCode + Fax;
        [Display(Name = "Date of Birth")]
        [NotMapped]
        public string DateOfBirthReport => DateOfBirth?.ToString("yyyy/MM/dd") ?? "";
        [Display(Name = "Date Joined")]
        [NotMapped]
        public string DateJoinedReport => DateJoined?.ToString("yyyy/MM/dd") ?? "";
        [Display(Name = "Groups")]
        [NotMapped]
        public string GroupsReport => string.Join(",", Groups.Select(g => g.Name));
        [Display(Name = "Capabilities")]
        [NotMapped]
        public string CapabilitiesReport => string.Join(", ", Capabilities.Select(g => $"{g.Skill.Name} {g.From.Name}" + (g.ToId.HasValue ? $" to {g.To.Name}" : "")));
        [Display(Name = "Languages")]
        [NotMapped]
        public string LanguagesReport => string.Join(",", Languages.Select(g => g.Name));
        [Display(Name = "Street address")]
        [NotMapped]
        public string StreetAddressReport => StreetAddress.ToString();
        [Display(Name = "Work address")]
        [NotMapped]
        public string WorkAddressReport => WorkAddress.ToString();

        [DisplayName("Tel (w)")]
        public string WorkPhone { get; set; }
        public string CellPhoneCode { get; set; }

        [Display(Name = "Cell")]
        public string CellPhone { get; set; }

        public string AlternatePhoneCode { get; set; }

        [Display(Name = "Alternate No")]
        public string AlternatePhone { get; set; }
        public string FaxCode { get; set; }
        public string Fax { get; set; }

        [Display(Name = "General Notes")]
        public string GeneralNotes { get; set; }

        [Display(Name = "Experience")]
        public string ExperienceNotes { get; set; }

        [Display(Name = "Hours Per Week Available")]
        public int HoursPerWeekAvailable { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
        //public bool Accredited { get; set; }
        public string Qualifications { get; set; }
        [Display(Name = "Works At Agency")]
        public bool WorksAtAgency { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public MemberType MemberType { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string Website { get; set; }
        [Display(Name = "Availability")]
        public AvailabilityType AvailabilityType { get; set; }
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Joined")]
        public DateTime? DateJoined { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedReport => LastUpdated?.ToString("yyyy/MM/dd") ?? "";
        [Display(Name = "Membership Year")]
        public int? MembershipYear { get; set; }

        public Address WorkAddress { get; set; }
        public int WorkAddressId { get; set; }

        public Address StreetAddress { get; set; }
        public int StreetAddressId { get; set; }

        [DisplayName("Area")]
        public int? AreaId { get; set; }
        public Area Area { get; set; }

        [DisplayName("Region")]
        public int? RegionId { get; set; }
        public Region Region { get; set; }

        [DisplayName("Primary Language")]
        public int? FirstLanguageId { get; set; }
        [NotMapped]
        public Language FirstLanguage
        {
            get
            {
                return Languages.FirstOrDefault(l => l.LanguageId == FirstLanguageId);
            }
        }

        public List<UserSocialMediaLink> UserSocialMediaLinks { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Capability> Capabilities { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public void ModifyPropertiesIfCompany(int membershipNo)
        {
            MembershipNo = membershipNo;
            if (MemberType == MemberType.Company)
            {
                WorkAddress = new Address();
                StreetAddress = new Address();
            }
        }
    }
}