using System.Collections.Generic;

namespace SATI.Entities
{
    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Region> Regions { get; set; }
    }
}