using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
    public class UpdateRoleDto
    {
        [Required, MaxLength(100)]
        public required string RoleName { get; set; }
    }
}
