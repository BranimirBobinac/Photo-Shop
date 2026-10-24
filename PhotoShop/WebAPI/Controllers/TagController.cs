using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        public TagController(AppDbContext context, LogService logService) { _context = context; _logService = logService; }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Tag.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _context.Tag.FindAsync(id);
            if (t == null) return NotFound(new { message = $"Ne mogu pronaći oznaku id={id}." });
            return Ok(new TagDto { Id = t.Id, Name = t.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.Tag.AnyAsync(t => t.Name == dto.Name))
                return BadRequest(new { message = "Oznaka s tim nazivom već postoji." });
            var tag = new Tag { Name = dto.Name };
            _context.Tag.Add(tag);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Oznaka s id={tag.Id} je stvorena.");
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, new { tag.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TagCreateDto dto)
        {
            var t = await _context.Tag.FindAsync(id);
            if (t == null) return NotFound();
            if (await _context.Tag.AnyAsync(x => x.Name == dto.Name && x.Id != id))
                return BadRequest(new { message = "Oznaka s tim nazivom već postoji." });
            t.Name = dto.Name;
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Oznaka s id={id} je ažurirana.");
            return Ok(new { message = "Oznaka ažurirana." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _context.Tag.Include(x => x.PhotoTags).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            _context.PhotoTag.RemoveRange(t.PhotoTags);
            _context.Tag.Remove(t);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Oznaka s id={id} je obrisana.");
            return Ok(new { message = "Oznaka obrisana." });
        }
    }
}
