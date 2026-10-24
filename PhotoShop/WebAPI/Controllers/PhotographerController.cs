using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/photographer")]
    public class PhotographerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        public PhotographerController(AppDbContext context, LogService logService) { _context = context; _logService = logService; }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Photographer.Select(p => new PhotographerDto { Id = p.Id, Name = p.Name, Bio = p.Bio, Email = p.Email }).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.Photographer.FindAsync(id);
            if (p == null) return NotFound(new { message = $"Fotograf s id={id} nije pronađen." });
            return Ok(new PhotographerDto { Id = p.Id, Name = p.Name, Bio = p.Bio, Email = p.Email });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhotographerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.Photographer.AnyAsync(p => p.Name == dto.Name))
                return BadRequest(new { message = "Fotograf s tim imenom već postoji." });
            var p = new Photographer { Name = dto.Name, Bio = dto.Bio, Email = dto.Email };
            _context.Photographer.Add(p);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotograf s id={p.Id} je stvoren.");
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, new { p.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PhotographerCreateDto dto)
        {
            var p = await _context.Photographer.FindAsync(id);
            if (p == null) return NotFound();
            if (await _context.Photographer.AnyAsync(x => x.Name == dto.Name && x.Id != id))
                return BadRequest(new { message = "Fotograf s tim imenom već postoji." });
            p.Name = dto.Name; p.Bio = dto.Bio; p.Email = dto.Email;
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotograf s id={id} je ažuriran.");
            return Ok(new { message = "Fotograf ažuriran." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Photographer.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            if (p.Photos.Any()) return BadRequest(new { message = "Ne možete obrisati fotografa koji ima fotografije." });
            _context.Photographer.Remove(p);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotograf s id={id} je obrisan.");
            return Ok(new { message = "Fotograf obrisan." });
        }
    }
}
