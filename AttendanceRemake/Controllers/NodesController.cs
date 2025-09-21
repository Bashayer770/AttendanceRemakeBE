using AttendanceRemake.Models;
using AttendanceRemake.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace AttendanceRemake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly ILogger<NodesController> _logger;
        private IRepository _rep;
        public NodesController(ILogger<NodesController> logger, IRepository rep)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route(nameof(GetNodeList))]
        public async Task<ActionResult<List<Node>>> GetNodeList()
        {
            try
            {
                var nodes = await _rep.GetAsync<Node>();

                return Ok(nodes);
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }

        }
        [HttpGet]
        [Route(nameof(GetNodeBySerial))]
        public async Task<ActionResult<Node>> GetNodeBySerial(string serial)
        {
            try
            {
                var nodes = await _rep.GetByIdAsync<Node>(serial);

                return Ok(nodes);
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }
        }
        [HttpPost]
        [Route(nameof(AddNewNode))]
        public async Task<ActionResult<JsonObject>> AddNewNode(Node node)
        {
            try
            {
                await _rep.AddAsync<Node>(node);
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }
        }

        [HttpPut]
        [Route(nameof(UpdateNode) + "/{serial}")]
        public async Task<IActionResult> UpdateNode([FromRoute] string serial, [FromBody] Node node)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serial))
                    return BadRequest("Serial is required.");

                await _rep.UpdateAsync<Node>(serial, async existing =>
                {
                    existing.DescA = node.DescA;
                    existing.DescE = node.DescE;
                    existing.LocCode = node.LocCode;
                    existing.Floor = node.Floor;
                    await Task.CompletedTask;
                });

                await _rep.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }
        }




        [HttpDelete]
        [Route(nameof(DeleteNode) + "/{serial}")]
        public async Task<IActionResult> DeleteNode([FromRoute] string serial)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serial))
                    return BadRequest("Serial is required.");

                await _rep.DeleteAsync<Node>(n => n.SerialNo == serial);
                await _rep.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }
        }




        private ObjectResult ResponseError(Exception ex)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(0);

            var method = frame.GetMethod();

            string methodName = method.Name; // Method name
            string className = method.DeclaringType?.FullName; // Class name

            return StatusCode(500, $"Error: {ex.Message}\nException Type: {ex.GetType()}\nLocation: {className}");

        }
    }
}
