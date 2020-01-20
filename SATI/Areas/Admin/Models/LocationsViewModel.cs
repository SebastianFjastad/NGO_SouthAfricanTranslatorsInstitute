using System.Collections.Generic;
using SATI.Entities;

namespace SATI.Areas.Admin.Models
{
    public class LocationsViewModel
    {
        public LocationsViewModel()
        {
            Countries = new List<Country>();
            Regions = new List<Region>();
            Areas = new List<Area>();
        }

        public int? CountryId { get; set; }
        public int? RegionId { get; set; }

        public List<Country> Countries{ get; set; }
        public List<Region> Regions { get; set; }
        public List<Area> Areas { get; set; }
    }
}