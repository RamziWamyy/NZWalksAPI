using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalksAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Username { get; set; }
        public string Password { get; set; }
        [Column("Date_Created")]
        public required DateTime DateCreated { get; set; }
        public required bool IsActive { get; set; }

        public int RoleId { get; set; }
        public  Role Role{ get; set; }
    }
}
