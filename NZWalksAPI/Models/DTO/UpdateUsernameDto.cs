using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
    public class UpdateUsernameDto
    {
        [Required, MaxLength(100)]
        public string NewUsername { get; set; }
    }
}
