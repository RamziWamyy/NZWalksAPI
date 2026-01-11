using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
    public class RoleDto
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public required string RoleName { get; set; }
    }
}
