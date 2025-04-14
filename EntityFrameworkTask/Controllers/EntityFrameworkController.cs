using EntityFrameworkTask.Context;
using EntityFrameworkTask.Interfaces;
using EntityFrameworkTask.Models;
using EntityFrameworkTask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityFrameworkController(IEntityFrameworkService efService) : ControllerBase
    {
        private readonly IEntityFrameworkService _efService = efService;

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(string value)
        {
            await _efService.AddRecordAsync(value);
            return Ok("Record added successfully");
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(int id, string value)
        {
            await _efService.UpdateRecordAsync(id, value);
            return Ok("Record updated successfully");
        }

        [HttpGet]
        [Route("index")]
        public async Task<List<SimpleTable>> Index()
        {
            List<SimpleTable> response = await _efService.GetAllRecordsAsync();
            return response;
        }
    }
}
