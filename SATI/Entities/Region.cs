
using System.Collections.Generic;

namespace SATI.Entities
{
    public class Region
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<Area> Areas { get; set; }
        public override string ToString() => Name;
    }
}