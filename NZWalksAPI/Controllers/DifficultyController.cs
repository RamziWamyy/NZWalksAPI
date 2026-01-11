using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Data;
using NZWalksAPI.Models;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifficultyController : ControllerBase
    {
        private readonly NZWalksDbContext _context;
        public DifficultyController(NZWalksDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var difficultyModel = _context.Difficulties.ToList();

            var difficultyDto = new List<DifficultyDto>();

            foreach (var difficulty in difficultyModel)
            {
                difficultyDto.Add(new DifficultyDto()
                {
                    Id = difficulty.Id,
                    Name = difficulty.Name,
                });
            }
            return Ok(difficultyDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var difficultyModel = _context.Difficulties.Find(id);

            if(difficultyModel is null)
            {
                return NotFound();
            }
            else
            {
                var difficultyDto = new DifficultyDto()
                {
                    Id = difficultyModel.Id,
                    Name = difficultyModel.Name,
                };

                return Ok(difficultyDto);
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] AddDifficultyDto addDifficultyDto)
        {
            var difficultyModel = new Difficulty
            {
                Name = addDifficultyDto.Name,
            };

            _context.Difficulties.Add(difficultyModel);
            _context.SaveChanges();

            var difficultyDto = new DifficultyDto
            {
                Id = difficultyModel.Id,
                Name = difficultyModel.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = difficultyDto.Id }, difficultyDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateDifficulty(Guid id, UpdateDifficultyDto updateDifficultyDto)
        {
            var difficulty = _context.Difficulties.Find(id);

            if (difficulty is null)
            {
                return NotFound();
            }
            else
            {
                difficulty.Name = updateDifficultyDto.Name;

                _context.SaveChanges();

                var difficultyDto = new DifficultyDto
                {
                    Id = difficulty.Id,
                    Name = difficulty.Name,
                };

                return Ok(difficultyDto);
            }
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteDifficulty([FromRoute] Guid id)
        {
            var difficultyModel = _context.Difficulties.Find(id);

            if(difficultyModel is null)
            {
                return NotFound();
            }
            else
            {
                _context.Difficulties.Remove(difficultyModel);
                _context.SaveChanges();

                var difficultyDto = new DeleteDifficultyDto
                {
                    Id = difficultyModel.Id,
                    Message = $"Difficulty with ID {id} was deleted succesfully."
                };

                return Ok(difficultyDto);
            }

        }
    }
}
