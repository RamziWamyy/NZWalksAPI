using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly NZWalksDbContext _context;
        public WalksController(NZWalksDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var walkModel = _context.Walks.Include(w => w.Difficulty)
                                            .Include(w => w.Region)
                                            .ToList();
            var walkDto = new List<WalkDto>();

            foreach (var walk in walkModel)
            {
                walkDto.Add(new WalkDto
                {
                    Id = walk.Id,
                    Name = walk.Name,
                    Description = walk.Description,
                    LengthInKm = walk.LengthInKm,
                    WalkImageUrl = walk.WalkImageUrl,

                    Difficulty = new DifficultyDto
                    {
                        Id = walk.Difficulty.Id,
                        Name = walk.Difficulty.Name,
                    },

                    Region = new RegionDto
                    {
                        Id = walk.Region.Id,
                        Code = walk.Region.Code,
                        Name = walk.Region.Name,
                        RegionImageUrl = walk.Region.RegionImageUrl
                    }
                });
            }

            return Ok(walkDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var walkModel = _context.Walks.Include(w => w.Difficulty)
                                            .Include (w => w.Region)
                                            .FirstOrDefault(w => w.Id == id);

            if(walkModel == null)
            {
                return NotFound();
            }
            else
            {
                var walkDto = new WalkDto()
                {
                    Id = walkModel.Id,
                    Name = walkModel.Name,
                    Description = walkModel.Description,
                    LengthInKm = walkModel.LengthInKm,
                    WalkImageUrl = walkModel.WalkImageUrl,

                    Difficulty = new DifficultyDto
                    {
                        Id = walkModel.Difficulty.Id,
                        Name = walkModel.Difficulty.Name,
                    },

                    Region = new RegionDto
                    {
                        Id = walkModel.Region.Id,
                        Code = walkModel.Region.Code,
                        Name = walkModel.Region.Name,
                        RegionImageUrl = walkModel.Region.RegionImageUrl
                    }
                };
                return Ok(walkDto);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddWalkDto addWalkDto)
        {
            var region = _context.Regions.FirstOrDefault(r => r.Code == addWalkDto.RegionCode);

            if(region is null)
            {
                return BadRequest($"Region with code {addWalkDto.RegionCode} was not found");
            }
            else
            {
                var difficulty = _context.Difficulties.FirstOrDefault(d => d.Name == addWalkDto.DifficultyName);

                if(difficulty is null)
                {
                    return BadRequest($"Difficulty {addWalkDto.RegionCode} was not found");
                }
                else
                {
                    var walkModel = new Walk
                    {
                        Name = addWalkDto.Name,
                        Description = addWalkDto.Description,
                        LengthInKm = addWalkDto.LengthInKm,
                        WalkImageUrl = addWalkDto.WalkImageUrl,

                        RegionId = region.Id,
                        DifficultyId = difficulty.Id,
                    };

                    _context.Walks.Add(walkModel);
                    _context.SaveChanges();

                    var walkDto = new WalkDto
                    {
                        Id = walkModel.Id,
                        Name = walkModel.Name,
                        Description = walkModel.Description,
                        WalkImageUrl = walkModel.WalkImageUrl,

                        Difficulty = new DifficultyDto
                        {
                            Id = difficulty.Id,
                            Name = difficulty.Name,
                        },

                        Region = new RegionDto
                        {
                            Id = region.Id,
                            Code = region.Code,
                            Name = region.Name,
                            RegionImageUrl = region.RegionImageUrl
                        }
                    };

                    return CreatedAtAction(nameof(GetById), new { id = walkDto.Id }, walkDto);
                }
            }
        }
        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateWalk(Guid id, UpdateWalkDto updateWalkDto)
        {
            var walk = _context.Walks.Find(id);

            if (walk is null)
            {
                return NotFound();
            }
            else
            {
                var region = _context.Regions.FirstOrDefault(r => r.Code == updateWalkDto.RegionCode);

                if (region is null)
                {
                    return BadRequest($"Region with code: {updateWalkDto.RegionCode} was not found.");
                }
                else
                {
                    var difficulty = _context.Difficulties.FirstOrDefault(d => d.Name == updateWalkDto.DifficultyName);

                    if(difficulty is null)
                    {
                        return BadRequest($"Difficulty with name: {updateWalkDto.DifficultyName} not found");
                    }
                    else
                    {
                        // 4) Update walk fields
                        walk.Name = updateWalkDto.Name;
                        walk.Description = updateWalkDto.Description;
                        walk.LengthInKm = updateWalkDto.LengthInKm;
                        walk.WalkImageUrl = updateWalkDto.WalkImageUrl;
                        walk.RegionId = region.Id;
                        walk.DifficultyId = difficulty.Id;

                        _context.SaveChanges();

                        // 5) Return DTO (nested)
                        var walkDto = new WalkDto
                        {
                            Id = walk.Id,
                            Name = walk.Name,
                            Description = walk.Description,
                            LengthInKm = walk.LengthInKm,
                            WalkImageUrl = walk.WalkImageUrl,

                            Difficulty = new DifficultyDto
                            {
                                Id = difficulty.Id,
                                Name = difficulty.Name
                            },

                            Region = new RegionDto
                            {
                                Id = region.Id,
                                Code = region.Code,
                                Name = region.Name,
                                RegionImageUrl = region.RegionImageUrl
                            }
                        };

                        return Ok(walkDto);
                    }
                }
            }
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteWalk([FromRoute] Guid id)
        {
            var walkModel = _context.Walks.Find(id);

            if (walkModel is null)
            {
                return NotFound();
            }

            _context.Walks.Remove(walkModel);
            _context.SaveChanges();

            var walkDto = new DeleteWalkDto
            {
                Id = walkModel.Id,
                Message = $"Walk with ID: {id} was deleted successfully."
            };

            return Ok(walkDto);
        }


    }
}
