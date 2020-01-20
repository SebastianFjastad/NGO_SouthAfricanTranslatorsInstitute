using System.Collections.Generic;
using SATI.Entities;

namespace SATI.Models
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Languages = new List<Language>();
            Skills = new List<Skill>();
            Accreditations = new List<Accreditation>();
            Specialisations = new List<Specialisation>();
            Users = new List<User>();
        }

        public List<Language> Languages { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Accreditation> Accreditations { get; set; }
        public List<Specialisation> Specialisations { get; set; }

        public List<User> Users { get; set; }

        public string SearchTerm { get; set; }
        public int? FromLanguageId { get; set; }
        public int? ToLanguageId { get; set; }
        public int? SkillId { get; set; }
        public int? AccreditationId { get; set; }
        public int? SpecialisationId { get; set; }
    }
}