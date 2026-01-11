using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
    public class UpdatePasswordDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
