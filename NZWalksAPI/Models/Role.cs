using NZWalksAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public required string RoleName { get; set; }

        public ICollection<User> Users { get; set; }

    }
}
