namespace NZWalksAPI.Models.DTO
{
    public class AddRegionDto
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? RegionImageUrl { get; set; }  //Adding the ? after the property type, means it can be NULLABLE!
    }
}
