using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalksAPI.Models.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Username { get; set; }
        public required DateTime DateCreated { get; set; }
        public required bool IsActive { get; set; }
        public required RoleDto Role { get; set; }
    }
}
