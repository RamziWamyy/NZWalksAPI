using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Data;
using NZWalksAPI.Models;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        //below via the param of the ctor we are injecting the DBContextClass

        private readonly NZWalksDbContext _context;
        public RegionsController(NZWalksDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] bool isAscending = true)
        {
            // Start with IQueryable so filtering/sorting happens in SQL
            var query = _context.Regions.AsQueryable();

            // FILTERING
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(r => r.Name.Contains(filterQuery));
                }
                else if (filterOn.Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(r => r.Code.Contains(filterQuery));
                }
            }

            // SORTING
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ? query.OrderBy(r => r.Name) : query.OrderByDescending(r => r.Name);
                }
                else if (sortBy.Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ? query.OrderBy(r => r.Code) : query.OrderByDescending(r => r.Code);
                }
            }

            // Execute query
            var regionsModel = query.ToList();

            // Map to DTOs
            var regionsDto = regionsModel.Select(region => new RegionDto
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            }).ToList();

            return Ok(regionsDto);
        }


        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute]Guid id)
        {

            //Use the Find() method when you want to filter by ID.
            var regionModel = _context.Regions.Find(id);

            //use the FirstOrDefault() method when you want to filter by any property of a table.
            //var region = _context.Regions.FirstOrDefault(x => x.Id == id);

            if (regionModel == null)
            {
                return NotFound();
            }
            else
            {
                //Map/Convert Domain Model to Region DTO.
                var regionDto = new RegionDto()
                {
                    Id = regionModel.Id,
                    Code = regionModel.Code,
                    Name = regionModel.Name,
                    RegionImageUrl = regionModel.RegionImageUrl
                };
                return Ok(regionDto);
            }
        }
        [HttpPost]
        //We use FromBody as a parameter as we need input from the user in order to add a new region.
        public IActionResult Create([FromBody] AddRegionDto addRegionDto)
        {
            var regionmodel = new Region
            {
                Code = addRegionDto.Code,
                Name = addRegionDto.Name,
                RegionImageUrl = addRegionDto.RegionImageUrl
            };


            _context.Regions.Add(regionmodel);
            _context.SaveChanges();

            var regionDto = new RegionDto
            {
                Id = regionmodel.Id,
                Code = regionmodel.Code,
                Name = regionmodel.Name,
                RegionImageUrl = regionmodel.RegionImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateRegion(Guid id, UpdateRegionDto updateRegionDto)
        {
            var region = _context.Regions.Find(id);

            if (region is null)
            {
                return NotFound();
            }
            else
            {
                region.Code = updateRegionDto.Code;
                region.Name = updateRegionDto.Name;
                region.RegionImageUrl = updateRegionDto.RegionImageUrl;

                _context.SaveChanges();

                var regionDto = new RegionDto
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                };

                return Ok(regionDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteRegion([FromRoute] Guid id)
        {

            var regionModel = _context.Regions.Find(id);

            if (regionModel is null)
            {
                return NotFound();
            }
            else
            {
                _context.Regions.Remove(regionModel);
                _context.SaveChanges();

                var regionDto = new DeleteRegionDto
                {
                    Id = regionModel.Id,
                    Message = $"Region with ID {id} was deleted successfully."
                };

                return Ok(regionDto);

            }
        }
    }
}
