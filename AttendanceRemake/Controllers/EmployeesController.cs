using System.Linq.Expressions;
using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using AttendanceRemake.Resources.APIs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceRemake.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly DB2Data _db2;

        public EmployeesController(IRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _db2 = new DB2Data(configuration);
        }

        // GET: api/Employees/search?empNo=&fingerCode=&civilId=&deptCode=&name=
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? empNo,
            [FromQuery] int? fingerCode,
            [FromQuery] string? civilId,
            [FromQuery] int? deptCode,
            [FromQuery] string? name)
        {
            if (!string.IsNullOrWhiteSpace(civilId))
            {
                var db2User = await _db2.GetUserByID(civilId);
                if (db2User == null) return NotFound("No DB2 user found for given CivilID.");
                if (!empNo.HasValue && db2User.EmpNo.HasValue) empNo = db2User.EmpNo.Value;
                if (!deptCode.HasValue && db2User.DeptCode.HasValue) deptCode = db2User.DeptCode.Value;
            }

            var q = (await _repo.GetAsync<Employee>()).AsQueryable();

            if (empNo.HasValue) q = q.Where(e => e.EmpNo == empNo.Value);
            if (fingerCode.HasValue) q = q.Where(e => e.FingerCode == fingerCode.Value);
            if (deptCode.HasValue) q = q.Where(e => e.DeptCode == deptCode.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var tokens = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (tokens.Length > 0)
                {
                    var param = Expression.Parameter(typeof(Employee), "e");
                    var nameAProp = Expression.Property(param, nameof(Employee.NameA));
                    var nameEProp = Expression.Property(param, nameof(Employee.NameE));

                    var efFuncs = Expression.Property(
                        null,
                        typeof(EF).GetProperty(nameof(EF.Functions))!
                    );

                    var likeMethod = typeof(DbFunctionsExtensions)
                        .GetMethods()
                        .First(m => m.Name == nameof(DbFunctionsExtensions.Like)
                                    && m.GetParameters().Length == 3
                                    && m.GetParameters()[1].ParameterType == typeof(string)
                                    && m.GetParameters()[2].ParameterType == typeof(string));

                    Expression? andChainA = null;
                    foreach (var token in tokens)
                    {
                        var patternConst = Expression.Constant($"%{token}%", typeof(string));
                        var likeA = Expression.Call(likeMethod, efFuncs, nameAProp, patternConst);
                        andChainA = andChainA == null ? likeA : Expression.AndAlso(andChainA, likeA);
                    }

                    Expression? andChainE = null;
                    foreach (var token in tokens)
                    {
                        var patternConst = Expression.Constant($"%{token}%", typeof(string));
                        var likeE = Expression.Call(likeMethod, efFuncs, nameEProp, patternConst);
                        andChainE = andChainE == null ? likeE : Expression.AndAlso(andChainE, likeE);
                    }

                    Expression orChain = andChainA!;
                    if (andChainE != null)
                    {
                        orChain = Expression.OrElse(andChainA!, andChainE);
                    }

                    var lambda = Expression.Lambda<Func<Employee, bool>>(orChain, param);
                    q = q.Where(lambda);
                }
            }

            var employees = await q.Take(200).ToListAsync();
            if (employees.Count == 0) return NotFound();

            UserData? providedCivilDb2 = null;
            if (!string.IsNullOrWhiteSpace(civilId))
                providedCivilDb2 = await _db2.GetUserByID(civilId);

            var result = employees.Select(e => new EmployeeDetailsDto
            {
                EmpNo = e.EmpNo,
                FingerCode = e.FingerCode,
                DeptCode = e.DeptCode,
                NameA = e.NameA,
                NameE = e.NameE,
                TimingCode = e.TimingCode,
                JobType = e.JobType,
                Sex = e.Sex,
                CheckLate = e.CheckLate,
                HasAllow = e.HasAllow,
                Status = e.Status,
                InLeave = e.InLeave,
                HasPass = e.HasPass,
                LocCode = e.LocCode,
                RegNo = e.RegNo,
                CivilID = providedCivilDb2?.CivilID,
                DB2DeptCode = providedCivilDb2?.DeptCode,
                FullName = providedCivilDb2?.FullName,
                ShortName = providedCivilDb2?.ShortName,
                MobileNo = providedCivilDb2?.MobileNo
            }).ToList();

            return Ok(result);
        }

        // GET: api/Employees/{empNo}
        [HttpGet("{empNo:int}")]
        public async Task<IActionResult> GetByEmpNo(int empNo)
        {
            var e = await _repo.GetByAsync<Employee>(x => x.EmpNo == empNo);
            if (e == null) return NotFound();

            return Ok(new EmployeeDetailsDto
            {
                EmpNo = e.EmpNo,
                FingerCode = e.FingerCode,
                DeptCode = e.DeptCode,
                NameA = e.NameA,
                NameE = e.NameE,
                TimingCode = e.TimingCode,
                JobType = e.JobType,
                Sex = e.Sex,
                CheckLate = e.CheckLate,
                HasAllow = e.HasAllow,
                Status = e.Status,
                InLeave = e.InLeave,
                HasPass = e.HasPass,
                LocCode = e.LocCode,
                RegNo = e.RegNo
            });
        }

        // PUT: api/Employees/{empNo}
        [HttpPut("{empNo:int}")]
        public async Task<IActionResult> UpdateEmployee(int empNo, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync<Employee>(empNo);
            if (existing == null) return NotFound($"Employee {empNo} not found.");

            await _repo.UpdateAsync<Employee>(empNo, async e =>
            {
                if (dto.FingerCode.HasValue) e.FingerCode = dto.FingerCode.Value;
                if (dto.TimingCode.HasValue) e.TimingCode = dto.TimingCode.Value;
                if (dto.HasAllow.HasValue) e.HasAllow = dto.HasAllow.Value;
                if (dto.Status.HasValue) e.Status = dto.Status.Value;
                await Task.CompletedTask;
            });

            return Ok(new
            {
                EmpNo = empNo,
                existing.FingerCode,
                existing.TimingCode,
                existing.HasAllow,
                existing.Status
            });
        }
    }

    public class UpdateEmployeeDto
    {
        public int? FingerCode { get; set; }
        public int? TimingCode { get; set; }
        public bool? HasAllow { get; set; }
        public int? Status { get; set; } // 0 or 1
    }

    public class EmployeeDetailsDto
    {
        // Employee fields
        public int EmpNo { get; set; }
        public int FingerCode { get; set; }
        public int DeptCode { get; set; }
        public string NameA { get; set; } = null!;
        public string NameE { get; set; } = null!;
        public int TimingCode { get; set; }
        public int JobType { get; set; }
        public short Sex { get; set; }
        public short CheckLate { get; set; }
        public bool HasAllow { get; set; }
        public int Status { get; set; }
        public bool InLeave { get; set; }
        public bool? HasPass { get; set; }
        public int? LocCode { get; set; }
        public int? RegNo { get; set; }

        // DB2 overlay
        public string? CivilID { get; set; }
        public int? DB2DeptCode { get; set; }
        public string? FullName { get; set; }
        public string? ShortName { get; set; }
        public int? MobileNo { get; set; }
    }
}