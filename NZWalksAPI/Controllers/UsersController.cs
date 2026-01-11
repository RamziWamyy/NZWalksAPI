using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NZWalksAPI.Data;
using NZWalksAPI.Models;
using NZWalksAPI.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly NZWalksDbContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(NZWalksDbContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Domain models + include Role because DTO needs it
            var userModel = _context.Users
                .Include(u => u.Role)
                .ToList();

            // Map to DTOs
            var userDto = new List<UserDto>();

            foreach (var user in userModel)
            {
                userDto.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    DateCreated = user.DateCreated,
                    IsActive = user.IsActive,

                    Role = new RoleDto
                    {
                        Id = user.Role.Id,
                        RoleName = user.Role.RoleName
                    }
                });
            }

            return Ok(userDto);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AddUserDto addUserDto)
        {
            //first checking if username is unique.
            var usernameExists = _context.Users.Any(u => u.Username == addUserDto.Username);

            if (usernameExists)
            {
                return BadRequest($"Username already exists.");
            }
            else
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == addUserDto.RoleId);

                if (role is null)
                {
                    return BadRequest($"Role with Id {addUserDto.RoleId} was not found.");
                }
                else
                {
                    //Create user model (no password stored)
                    var user = new User
                    {
                        Username = addUserDto.Username,
                        RoleId = role.Id,
                        DateCreated = DateTime.UtcNow,
                        IsActive = addUserDto.IsActive
                    };

                    // Hash password securely
                    var hasher = new PasswordHasher<User>();
                    user.Password = hasher.HashPassword(user, addUserDto.Password);

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var userDto = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        DateCreated = user.DateCreated,
                        IsActive = user.IsActive,
                        Role = new RoleDto
                        {
                            Id = role.Id,
                            RoleName = role.RoleName
                        }
                    };

                    return Ok(userDto);
                }
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _context.Users.Include(u => u.Role)
                                    .FirstOrDefault(u => u.Username == loginDto.Username);
            if (user is null)
            {
                return Unauthorized("Invalid username or password");
            }
            else
            {
                if(!user.IsActive)
                {
                    return Unauthorized("User account was delted!");
                }
                else
                {
                    //verify password
                    var hasher = new PasswordHasher<User>();
                    var result = hasher.VerifyHashedPassword(
                        user,
                        user.Password,
                        loginDto.Password);

                    if(result == PasswordVerificationResult.Failed)
                    {
                        return Unauthorized("Invalid username or password!");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.Role.RoleName)
                        };

                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(2),
                            signingCredentials: creds
                        );

                        return Ok(new
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token)
                        });
                    }
                }
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("{id:int}/username")]
        public IActionResult UpdateUsername([FromRoute] int id, [FromBody] UpdateUsernameDto updateUsernameDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user is null)
            {
                return NotFound();
            }
            else
            {
                var usernameTaken = _context.Users.Any(u => u.Username == updateUsernameDto.NewUsername && u.Id != id);

                if (usernameTaken)
                {
                    return BadRequest("Username already exists.");
                }
                else
                {
                    user.Username = updateUsernameDto.NewUsername;
                    _context.SaveChanges();

                    return Ok(new { Message = $"Username updated for user {id}." });
                }
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("{id:int}/password")]
        public IActionResult ChangePassword([FromRoute] int id, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if(user is null)
            {
                return NotFound();
            }
            else
            {
                if(!user.IsActive)
                {
                    return BadRequest("User is inactive");
                }
                else
                {
                    var hasher = new PasswordHasher<User>();
                    user.Password = hasher.HashPassword(user, updatePasswordDto.NewPassword);

                    _context.SaveChanges();

                    return Ok(new { Message = $"Password updated for user {id}." });
                }
            }
        }
    }
}
