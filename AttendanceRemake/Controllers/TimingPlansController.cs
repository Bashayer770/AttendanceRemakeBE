using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceRemake.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimingPlansController : ControllerBase
    {
        private readonly IRepository _repo;
        public TimingPlansController(IRepository repo) => _repo = repo;

        // GET: api/TimingPlans
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var q = await _repo.GetAsync<TimingPlan>();
            var list = await q.Select(tp => new TimingPlanDto
            {
                Code = tp.Code,
                DescA = tp.DescA,
                DescE = tp.DescE
            }).ToListAsync();
            return Ok(list);
        }

        // GET: api/TimingPlans/{code}
        [HttpGet("{code:int}")]
        public async Task<IActionResult> GetByCode(int code)
        {
            var tp = await _repo.GetByAsync<TimingPlan>(x => x.Code == code);
            if (tp == null) return NotFound();
            return Ok(new TimingPlanDto { Code = tp.Code, DescA = tp.DescA, DescE = tp.DescE });
        }
    }

    public class TimingPlanDto
    {
        public int Code { get; set; }
        public string DescA { get; set; } = null!;
        public string DescE { get; set; } = null!;
    }
}