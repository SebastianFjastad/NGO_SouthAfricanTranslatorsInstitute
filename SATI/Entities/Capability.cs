using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SATI.Entities
{
    public class Capability
    {
        public Capability()
        {
            Accreditations = new List<Accreditation>();
            Specialisations = new List<Specialisation>();
        }

        public int CapabilityId { get; set; }

        public int? Unique_ID { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public int FromId { get; set; }
        [ForeignKey("FromId")]
        public Language From { get; set; }

        public int? ToId { get; set; }
        [ForeignKey("ToId")]
        public Language To { get; set; }

        [DisplayName("Is Accredited")]
        public bool IsAccredited { get; set; }

        public List<Accreditation> Accreditations { get; set; }
        public List<Specialisation> Specialisations { get; set; }
    }
}