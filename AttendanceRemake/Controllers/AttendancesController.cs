using AttendanceRemake.Business.Attendance;
using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using AttendanceRemake.Resources.APIs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRemake.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly ILogger<AttendancesController> _logger;
        private IRepository _rep;
        private DB2Data _db2;
        public AttendancesController(ILogger<AttendancesController> logger, IRepository rep, IConfiguration configuration)
        {
            _logger = logger;
            _rep = rep;
            _db2 = new DB2Data(configuration);
        }
        [HttpGet]
        [Route(nameof(GetAttendanceRecord))]
        public ActionResult<List<Attendance>> GetAttendanceRecord(DateTime startDate, DateTime EndDate)
        {
            try
            {
                ValidateInput(startDate, EndDate);

                UserData user = _db2.GetUserByID("ebs037").Result;

                int fingerCode = _rep.GetByIdAsync<Employee>(user.EmpNo).Result.FingerCode;
                var record = _rep.GetListByAsync<Attendance>(con => con.FingerCode == fingerCode && con.IodateTime > startDate && con.IodateTime < EndDate).Result.ToList<Attendance>();

                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route(nameof(GetLateRecord))]
        public ActionResult<List<AttendanceLog>> GetLateRecord(string user, DateTime startDate, DateTime EndDate)
        {
            try
            {
                ValidateInput(startDate, EndDate);
                AttendanceBusiness ab = new AttendanceBusiness();
                UserData userData = _db2.GetUserByID(user).Result;

                Employee empInfo = _rep.GetByIdAsync<Employee>(userData.EmpNo).Result;

                var userActivity = _rep.GetListByAsync<Attendance>(con => con.FingerCode == empInfo.FingerCode && con.IodateTime.Date >= startDate.Date && con.IodateTime.Date <= EndDate.Date && (con.TrType == 1 || con.TrType == 0)  ).Result.OrderBy(c => c.IodateTime).ToList<Attendance>();
                TimingPlan userTimePlan = _rep.GetByAsync<TimingPlan>(tp => tp.Code == empInfo.TimingCode).Result;


                var record = ab.GetUserActivity(userActivity, userTimePlan);

                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private void ValidateInput(DateTime startDate, DateTime EndDate)
        {
            if ((EndDate - startDate).TotalDays > 92)
            {
                throw new Exception("Date Range cannot be more than 3 months");
            }
            if (EndDate < startDate)
            {
                throw new Exception("Starting date cannot be greater then the end date");
            }
        }

    }
}
