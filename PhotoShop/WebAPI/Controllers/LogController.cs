using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LogController(AppDbContext context) { _context = context; }

        [HttpGet("get/{n}")]
        public async Task<IActionResult> GetLastN(int n)
        {
            var logs = await _context.Log.OrderByDescending(l => l.Timestamp).Take(n).ToListAsync();
            return Ok(logs);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count() =>
            Ok(new { count = await _context.Log.CountAsync() });
    }
}
