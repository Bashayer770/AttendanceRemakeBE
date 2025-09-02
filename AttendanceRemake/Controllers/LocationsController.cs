using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRemake.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly IRepository _repo;

        public LocationsController(IRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _repo.GetAsync<Location>();
            return Ok(locations);
        }

        // GET: api/Locations/5
        [HttpGet("{code}")]
        public async Task<IActionResult> GetLocationByCode(int code)
        {
            var location = await _repo.GetByIdAsync<Location>(code);
            if (location == null)
                return NotFound();

            return Ok(location);
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Location location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var MaxId = _repo.GetAsync<Location>().Result.Max(e => (int?)e.Code) ?? 1;
            location.Code = MaxId+1;


            var success = await _repo.AddAsync(location);
            if (!success)
                return StatusCode(500, "Failed to create location");

            await _repo.SaveAsync();
            return Ok(location); 
        }



        // PUT: api/Locations/
        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateLocation(int code, [FromBody] Location model)
        {
            await _repo.UpdateAsync<Location>(code, async existing =>
            {
                existing.DescA = model.DescA;
                existing.DescE = model.DescE;
                await Task.CompletedTask;
            });

            await _repo.SaveAsync();
            return Ok();
        }

        // GET: api/Locations/search?query
        [HttpGet("search")]
        public async Task<IActionResult> SearchLocations(string query)
        {
            var results = await _repo.GetListByAsync<Location>(
                x => x.DescA.Contains(query) || x.DescE.Contains(query)
            );

            return Ok(results);
        }


        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(int code)
        {
            var location = await _repo.GetByAsync<Location>(l => l.Code == code);
            if (location == null)
                return NotFound();

            await _repo.DeleteAsync<Location>(l => l.Code == code);
            await _repo.SaveAsync();

            return NoContent(); 
        }


    }
}