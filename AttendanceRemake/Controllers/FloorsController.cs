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


        

    }
}
