using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SATI.Entities
{
    public class Specialisation
    {
        public Specialisation()
        {
            Capabilities = new List<Capability>();
        }
        public int SpecialisationId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Capability> Capabilities { get; set; }
    }
}