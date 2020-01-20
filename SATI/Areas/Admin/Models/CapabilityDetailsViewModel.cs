using System.Collections.Generic;
using System.Linq;
using SATI.Entities;

namespace SATI.Areas.Admin.Models
{
    public class CapabilityDetailsViewModel
    {
        public CapabilityDetailsViewModel(Capability cap, string memberId, List<Specialisation> allSpecialisations,
            List<Accreditation> allAccreditations)
        {
            Capability = cap;
            MemberId = memberId;
            UnselectedSpecialisations =
                allSpecialisations.Where(a => cap.Specialisations.All(cs => cs.SpecialisationId != a.SpecialisationId))
                    .ToList();

            SelectedSpecialisations = cap.Specialisations.ToList();

            UnselectedAccreditations =
                allAccreditations.Where(aa => cap.Accreditations.All(ca => ca.AccreditationId != aa.AccreditationId))
                    .ToList();

            SelectedAccreditations = cap.Accreditations.ToList();
        }

        public Capability Capability { get; set; }

        public string MemberId { get; set; }

        public List<Specialisation> UnselectedSpecialisations { get; set; }
        public List<Specialisation> SelectedSpecialisations { get; set; }

        public List<Accreditation> UnselectedAccreditations { get; set; }
        public List<Accreditation> SelectedAccreditations { get; set; }

    }
}