using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.Tag.ToListAsync());
        public IActionResult Create() => View();

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _context.Tag.AnyAsync(t => t.Name == model.Name)) { ModelState.AddModelError("Name", "Oznaka s tim nazivom već postoji."); return View(model); }
            _context.Tag.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id) { var t = await _context.Tag.FindAsync(id); if (t == null) return NotFound(); return View(t); }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tag model)
        {
            if (!ModelState.IsValid) return View(model);
            var t = await _context.Tag.FindAsync(id);
            if (t == null) return NotFound();
            if (await _context.Tag.AnyAsync(x => x.Name == model.Name && x.Id != id)) { ModelState.AddModelError("Name", "Oznaka s tim nazivom već postoji."); return View(model); }
            t.Name = model.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id) { var t = await _context.Tag.FindAsync(id); if (t == null) return NotFound(); return View(t); }

        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _context.Tag.Include(x => x.PhotoTags).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            _context.PhotoTag.RemoveRange(t.PhotoTags);
            _context.Tag.Remove(t);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
