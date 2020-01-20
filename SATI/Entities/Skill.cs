using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SATI.Entities
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsTransitive { get; set; }
    }
}