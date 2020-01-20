using System.Collections.Generic;
using System.Linq;
using SATI.Entities;
using SATI.Models;

namespace SATI.Areas.Admin.Models
{
    public class CapabilitiesViewModel
    {
        public CapabilitiesViewModel()
        {
            User = new User();
            AllSkills = new List<Skill>();
            Languages = new List<Language>();
        }

        public CapabilitiesViewModel(User member, List<Skill> allSkills)
        {
            User = member;
            AllSkills = allSkills;
            Languages = member?.Languages.ToList();
        }

        public Capability Capability { get; set; }

        public User User { get; set; }
        
        public List<Skill> AllSkills { get; set; }
        public List<Language> Languages { get; set; }
    }
}