using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
    public class AddRoleDto
    {
        [Required, MaxLength(100)]
        public required string RoleName { get; set; }
    }
}
