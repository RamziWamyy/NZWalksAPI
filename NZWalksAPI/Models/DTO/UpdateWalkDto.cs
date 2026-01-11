namespace NZWalksAPI.Models.DTO
{
    public class UpdateWalkDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        public required string RegionCode { get; set; }       // e.g. "NA"
        public required string DifficultyName { get; set; }   // e.g. "Easy"
    }
}
