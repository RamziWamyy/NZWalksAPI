using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Data;
using NZWalksAPI.Models;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly NZWalksDbContext _context;

        public RolesController(NZWalksDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roleModel = _context.Roles.ToList();

            var roleDto = new List<RoleDto>();

            foreach (var role in roleModel)
            {
                roleDto.Add(new RoleDto()
                {
                    Id = role.Id,
                    RoleName = role.RoleName
                });
            }
            return Ok(roleDto);
        }
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {

            //Use the Find() method when you want to filter by ID.
            var roleModel = _context.Roles.Find(id);

            //use the FirstOrDefault() method when you want to filter by any property of a table.
            //var region = _context.Regions.FirstOrDefault(x => x.Id == id);

            if (roleModel == null)
            {
                return NotFound();
            }
            else
            {
                //Map/Convert Domain Model to Region DTO.
                var roleDto = new RoleDto()
                {
                    Id = roleModel.Id,
                    RoleName = roleModel.RoleName
                    
                };
                return Ok(roleDto);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddRoleDto addRoleDto)
        {
            var roleModel = new Role
            {
                RoleName = addRoleDto.RoleName
            };

            _context.Roles.Add(roleModel);
            _context.SaveChanges();

            var roleDto = new RoleDto
            {
                Id = roleModel.Id,
                RoleName = roleModel.RoleName
            };

            return CreatedAtAction(nameof(GetById), new { id = roleDto.Id }, roleDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateRole(int id, UpdateRoleDto updateRoleDto)
        {
            var role = _context.Roles.Find(id);

            if(role is null)
            {
                return NotFound();
            }
            else
            {
                role.RoleName = updateRoleDto.RoleName;

                _context.SaveChanges();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    RoleName = role.RoleName
                };

                return Ok(roleDto);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteRole([FromRoute] int id)
        {
            var roleModel = _context.Roles.Find(id);

            if(roleModel is null)
            {
                return NotFound();
            }
            else
            {
                _context.Roles.Remove(roleModel);
                _context.SaveChanges();

                var roleDto = new DeleteRoleDto
                {
                    Id = roleModel.Id,
                    Message = $"Role with ID {id} was deleted succesfully."
                };

                return Ok(roleDto);
            }
        }
    }
}
