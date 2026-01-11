using Microsoft.AspNetCore.Identity;
using NZWalksAPI.Models;

namespace NZWalksAPI.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<User> _hasher = new();

        public static string HashPassword(User user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public static bool VerifyPassword(User user, string password, string storedHash)
        {
            var result = _hasher.VerifyHashedPassword(user, storedHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
