using ElasticSearchTask.Interfaces;
using ElasticSearchTask.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ElasticSearchTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController(ILogService logService) : ControllerBase
    {
        private readonly ILogService _logService = logService;

        [HttpPost("index")]
        public async Task<IActionResult> IndexLog([FromBody] LogEntry log)
        {
            try
            {
                if (log == null) return BadRequest("Log entry is required");

                await _logService.IndexLogAsync(log);
                return Ok(new { Message = "Log indexed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error indexing log", Error = ex.Message });
            }
        }

        [HttpPost("indexRandom")]
        public async Task<IActionResult> IndexRandomLog()
        {
            try
            {
                await _logService.IndexRandomLogAsync();
                return Ok(new { Message = "Random logs indexed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error indexing log", Error = ex.Message });
            }
        }

        [HttpDelete("removeAllLogs")]
        public async Task<IActionResult> RemoveAllLogs()
        {
            try
            {
                await _logService.RemoveAllLogData();
                return Ok(new { Message = "Log data deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error indexing log", Error = ex.Message });
            }
        }

        [HttpGet("searchAll")]
        public async Task<IActionResult> SearchLogs()
        {
            try
            {
                var result = await _logService.SearchLogsAsync(null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error searching logs", Error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchLogs([FromQuery] string query)
        {
            try
            {
                var result = await _logService.SearchLogsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error searching logs", Error = ex.Message });
            }
        }
    }
}