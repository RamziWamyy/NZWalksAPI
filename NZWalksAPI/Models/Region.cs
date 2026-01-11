namespace NZWalksAPI.Models
{
    public class Region
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }  //Adding the ? after the property type, means it can be NULLABLE!
    }
}
