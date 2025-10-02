using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRemake.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorsController : ControllerBase
    {
        private readonly IRepository _repo;

        public FloorsController(IRepository repo) => _repo = repo;

        // GET: api/Floors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var floors = await _repo.GetAsync<Floor>();
            return Ok(floors);
        }

        // GET: api/Floors/{floor}
        [HttpGet("{floor}")]
        public async Task<IActionResult> GetFloorById(string floor)
        {
            var floorById = await _repo.GetByIdAsync<Floor>(floor);
            if (floorById == null)
                return NotFound();

            return Ok(floorById);
        }



        // GET: api/Floors/search?query=...
        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _repo.GetListByAsync<Floor>(
                x => x.Floor1.Contains(query) || x.DescA.Contains(query) || x.DescB.Contains(query)
            );
            return Ok(results);
        }



        // POST: api/Floors
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Floor floor)
        {
            if (floor == null || string.IsNullOrWhiteSpace(floor.Floor1))
                return BadRequest("Floor key is required.");

            var exists = await _repo.AnyAsync<Floor>(f => f.Floor1 == floor.Floor1);
            if (exists)
                return Conflict($"Floor '{floor.Floor1}' already exists.");

            await _repo.AddAsync(floor);
            return CreatedAtAction(nameof(GetFloorById), new { floor = floor.Floor1 }, floor);
        }




        //public class CreateFloorDto
        //{
        //    public string DescA { get; set; } = null!;
        //    public string DescE { get; set; } = null!;
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateFloorDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var query = await _repo.GetAsync<Floor>();
        //    var maxId = query
        //        .AsEnumerable()
        //        .Select(f => int.TryParse(f.Floor1, out var n) ? n : (int?)null)
        //        .Where(n => n.HasValue)
        //        .Select(n => n!.Value)
        //        .DefaultIfEmpty(0)
        //        .Max();

        //    var entity = new Floor
        //    {
        //        Floor1 = (maxId + 1).ToString(),
        //        DescA = dto.DescA,
        //        DescB = dto.DescE
        //    };

        //    var success = await _repo.AddAsync(entity);
        //    if (!success) return StatusCode(500, "Failed to create floor");

        //    await _repo.SaveAsync();
        //    return Ok(entity);
        //}



        // PUT: api/Floors/{floor}
        [HttpPut("{floor}")]
        public async Task<IActionResult> Update(string floor, [FromBody] Floor payload)
        {
            if (string.IsNullOrWhiteSpace(floor))
                return BadRequest("Floor key is required.");

            await _repo.UpdateAsync<Floor>(floor, async existing =>
            {
                existing.DescA = payload.DescA;
                existing.DescB = payload.DescB;
                await Task.CompletedTask;
            });

            return Ok();
        }

        // DELETE: api/Floors/{floor}
        [HttpDelete("{floor}")]
        public async Task<IActionResult> Delete(string floor)
        {
            if (string.IsNullOrWhiteSpace(floor))
                return BadRequest("Floor key is required.");

            await _repo.DeleteAsync<Floor>(f => f.Floor1 == floor);
            return NoContent();
        }



    }
}
