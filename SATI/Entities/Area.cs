
namespace SATI.Entities
{
    public class Area
    {
        public int AreaId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public override string ToString() => Name;
    }
}