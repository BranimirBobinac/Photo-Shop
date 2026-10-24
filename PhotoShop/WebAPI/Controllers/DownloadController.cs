using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        public DownloadController(AppDbContext context, LogService logService) { _context = context; _logService = logService; }

        [HttpPost]
        public async Task<IActionResult> AddDownload([FromBody] DownloadRequestDto dto)
        {
            if (!await _context.Download.AnyAsync(d => d.PhotoId == dto.PhotoId && d.AppUserId == dto.UserId))
            {
                _context.Download.Add(new Download { PhotoId = dto.PhotoId, AppUserId = dto.UserId, DownloadedAt = DateTime.UtcNow });
                await _context.SaveChangesAsync();
                await _logService.LogAsync($"Korisnik id={dto.UserId} je preuzeo fotografiju id={dto.PhotoId}.");
            }
            return Ok(new { message = "Preuzimanje zabilježeno." });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDownloads(int userId)
        {
            var downloads = await _context.Download
                .Where(d => d.AppUserId == userId)
                .Include(d => d.Photo)
                .Select(d => new { d.Id, d.DownloadedAt, PhotoId = d.PhotoId, PhotoName = d.Photo!.Name, PhotoImageUrl = d.Photo.ImageUrl })
                .ToListAsync();
            return Ok(downloads);
        }
    }

    public class DownloadRequestDto
    {
        public int PhotoId { get; set; }
        public int UserId { get; set; }
    }
}
