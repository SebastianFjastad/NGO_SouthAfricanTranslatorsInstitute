using System.Collections.Generic;
using System.Linq;
using SATI.Entities;
using SATI.Models;

namespace SATI.Areas.Admin.Models
{
    public class LanguagesViewModel
    {
        public LanguagesViewModel()
        {
                
        }

        public LanguagesViewModel(List<Language> allLanguages, User user)
        {
            Languages = allLanguages.Where(u => user.Languages.All(x => x.LanguageId != u.LanguageId)).ToList();
            User = user;
        }

        public User User { get; set; }
        public List<Language> Languages { get; set; }
    }
}