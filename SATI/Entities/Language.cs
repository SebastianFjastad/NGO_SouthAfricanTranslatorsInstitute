using SATI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SATI.Entities
{
    public class Language
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}