using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/photo")]
    public class PhotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public PhotoController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var photos = await _context.Photo
                .Include(p => p.Photographer)
                .Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag)
                .Select(p => new PhotoDto
                {
                    Id = p.Id, Name = p.Name, Description = p.Description,
                    ImageUrl = p.ImageUrl, Price = p.Price, CreatedAt = p.CreatedAt,
                    PhotographerId = p.PhotographerId,
                    PhotographerName = p.Photographer != null ? p.Photographer.Name : null,
                    Tags = p.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
                }).ToListAsync();
            return Ok(photos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var photo = await _context.Photo
                .Include(p => p.Photographer)
                .Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null)
            {
                await _logService.LogAsync($"Fotografija s id={id} nije pronađena.", "Warning");
                return NotFound(new { message = $"Fotografija s id={id} nije pronađena." });
            }
            return Ok(new PhotoDto
            {
                Id = photo.Id, Name = photo.Name, Description = photo.Description,
                ImageUrl = photo.ImageUrl, Price = photo.Price, CreatedAt = photo.CreatedAt,
                PhotographerId = photo.PhotographerId, PhotographerName = photo.Photographer?.Name,
                Tags = photo.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? description, [FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            var query = _context.Photo.Include(p => p.Photographer).Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag).AsQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(p => p.Name.Contains(name));
            if (!string.IsNullOrEmpty(description)) query = query.Where(p => p.Description != null && p.Description.Contains(description));
            var total = await query.CountAsync();
            var photos = await query.Skip((page - 1) * count).Take(count)
                .Select(p => new PhotoDto
                {
                    Id = p.Id, Name = p.Name, Description = p.Description,
                    ImageUrl = p.ImageUrl, Price = p.Price, CreatedAt = p.CreatedAt,
                    PhotographerId = p.PhotographerId,
                    PhotographerName = p.Photographer != null ? p.Photographer.Name : null,
                    Tags = p.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
                }).ToListAsync();
            return Ok(new { total, page, count, data = photos });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhotoCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.Photo.AnyAsync(p => p.Name == dto.Name))
                return BadRequest(new { message = "Fotografija s tim nazivom već postoji." });
            var photo = new Photo { Name = dto.Name, Description = dto.Description, ImageUrl = dto.ImageUrl, Price = dto.Price, PhotographerId = dto.PhotographerId, CreatedAt = DateTime.UtcNow };
            _context.Photo.Add(photo);
            await _context.SaveChangesAsync();
            foreach (var tagId in dto.TagIds) _context.PhotoTag.Add(new PhotoTag { PhotoId = photo.Id, TagId = tagId });
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotografija s id={photo.Id} je stvorena.");
            return CreatedAtAction(nameof(GetById), new { id = photo.Id }, new { photo.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PhotoUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var photo = await _context.Photo.Include(p => p.PhotoTags).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null)
            {
                await _logService.LogAsync($"Došlo je do problema prilikom ažuriranja fotografije s id={id}.", "Warning");
                return NotFound(new { message = $"Fotografija s id={id} nije pronađena." });
            }
            if (await _context.Photo.AnyAsync(p => p.Name == dto.Name && p.Id != id))
                return BadRequest(new { message = "Fotografija s tim nazivom već postoji." });
            photo.Name = dto.Name; photo.Description = dto.Description; photo.ImageUrl = dto.ImageUrl;
            photo.Price = dto.Price; photo.PhotographerId = dto.PhotographerId;
            _context.PhotoTag.RemoveRange(photo.PhotoTags);
            foreach (var tagId in dto.TagIds) _context.PhotoTag.Add(new PhotoTag { PhotoId = id, TagId = tagId });
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotografija s id={id} je ažurirana.");
            return Ok(new { message = "Fotografija ažurirana." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _context.Photo.Include(p => p.PhotoTags).Include(p => p.Downloads).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null)
            {
                await _logService.LogAsync($"Došlo je do problema prilikom brisanja fotografije s id={id}.", "Warning");
                return NotFound(new { message = $"Fotografija s id={id} nije pronađena." });
            }
            _context.PhotoTag.RemoveRange(photo.PhotoTags);
            _context.Download.RemoveRange(photo.Downloads);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Fotografija s id={id} je obrisana.");
            return Ok(new { message = "Fotografija obrisana." });
        }
    }
}
