using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PhotographerController : Controller
    {
        private readonly AppDbContext _context;
        public PhotographerController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.Photographer.ToListAsync());
        public IActionResult Create() => View();

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Photographer model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _context.Photographer.AnyAsync(p => p.Name == model.Name)) { ModelState.AddModelError("Name", "Fotograf s tim imenom već postoji."); return View(model); }
            _context.Photographer.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id) { var p = await _context.Photographer.FindAsync(id); if (p == null) return NotFound(); return View(p); }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Photographer model)
        {
            if (!ModelState.IsValid) return View(model);
            var p = await _context.Photographer.FindAsync(id);
            if (p == null) return NotFound();
            if (await _context.Photographer.AnyAsync(x => x.Name == model.Name && x.Id != id)) { ModelState.AddModelError("Name", "Fotograf s tim imenom već postoji."); return View(model); }
            p.Name = model.Name; p.Bio = model.Bio; p.Email = model.Email;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id) { var p = await _context.Photographer.FindAsync(id); if (p == null) return NotFound(); return View(p); }

        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _context.Photographer.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            if (p.Photos.Any()) { TempData["Error"] = "Ne možete obrisati fotografa koji ima fotografije."; return RedirectToAction(nameof(Index)); }
            _context.Photographer.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
