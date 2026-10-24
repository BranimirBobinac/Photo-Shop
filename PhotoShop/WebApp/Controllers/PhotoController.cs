using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PhotoController : Controller
    {
        private readonly AppDbContext _context;
        public PhotoController(AppDbContext context) { _context = context; }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? searchText, int? photographerFilter, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Photo.Include(p => p.Photographer).Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(p => p.Name.Contains(searchText) || (p.Description != null && p.Description.Contains(searchText)));
            if (photographerFilter.HasValue)
                query = query.Where(p => p.PhotographerId == photographerFilter.Value);
            var total = await query.CountAsync();
            var photos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var vm = new PhotoListViewModel
            {
                Photos = photos.Select(p => new PhotoViewModel
                {
                    Id = p.Id, Name = p.Name, Description = p.Description, ImageUrl = p.ImageUrl,
                    Price = p.Price, CreatedAt = p.CreatedAt, PhotographerId = p.PhotographerId,
                    PhotographerName = p.Photographer?.Name,
                    TagNames = p.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
                }).ToList(),
                SearchText = searchText, PhotographerFilter = photographerFilter,
                Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name"),
                CurrentPage = page, TotalPages = (int)Math.Ceiling((double)total / pageSize), PageSize = pageSize
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View(new PhotoViewModel
            {
                Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name"),
                Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name")
            });
        }

        [HttpPost][Authorize(Roles = "Admin")][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhotoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name");
                vm.Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name");
                return View(vm);
            }
            if (await _context.Photo.AnyAsync(p => p.Name == vm.Name))
            {
                ModelState.AddModelError("Name", "Fotografija s tim nazivom već postoji.");
                vm.Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name");
                vm.Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name");
                return View(vm);
            }
            var photo = new Photo { Name = vm.Name, Description = vm.Description, ImageUrl = vm.ImageUrl, Price = vm.Price, PhotographerId = vm.PhotographerId, CreatedAt = DateTime.UtcNow };
            _context.Photo.Add(photo);
            await _context.SaveChangesAsync();
            foreach (var tagId in vm.SelectedTagIds) _context.PhotoTag.Add(new PhotoTag { PhotoId = photo.Id, TagId = tagId });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var photo = await _context.Photo.Include(p => p.PhotoTags).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            return View(new PhotoViewModel
            {
                Id = photo.Id, Name = photo.Name, Description = photo.Description, ImageUrl = photo.ImageUrl,
                Price = photo.Price, PhotographerId = photo.PhotographerId,
                SelectedTagIds = photo.PhotoTags.Select(pt => pt.TagId).ToList(),
                Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name"),
                Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name")
            });
        }

        [HttpPost][Authorize(Roles = "Admin")][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhotoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name");
                vm.Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name");
                return View(vm);
            }
            var photo = await _context.Photo.Include(p => p.PhotoTags).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            if (await _context.Photo.AnyAsync(p => p.Name == vm.Name && p.Id != id))
            {
                ModelState.AddModelError("Name", "Fotografija s tim nazivom već postoji.");
                vm.Photographers = new SelectList(await _context.Photographer.ToListAsync(), "Id", "Name");
                vm.Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name");
                return View(vm);
            }
            photo.Name = vm.Name; photo.Description = vm.Description; photo.ImageUrl = vm.ImageUrl;
            photo.Price = vm.Price; photo.PhotographerId = vm.PhotographerId;
            _context.PhotoTag.RemoveRange(photo.PhotoTags);
            foreach (var tagId in vm.SelectedTagIds) _context.PhotoTag.Add(new PhotoTag { PhotoId = id, TagId = tagId });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _context.Photo.Include(p => p.Photographer).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            return View(photo);
        }

        [HttpPost, ActionName("Delete")][Authorize(Roles = "Admin")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photo.Include(p => p.PhotoTags).Include(p => p.Downloads).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            _context.PhotoTag.RemoveRange(photo.PhotoTags);
            _context.Download.RemoveRange(photo.Downloads);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Browse(string? searchText, int? tagFilter, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Photo.Include(p => p.Photographer).Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(p => p.Name.Contains(searchText) || (p.Description != null && p.Description.Contains(searchText)));
            if (tagFilter.HasValue)
                query = query.Where(p => p.PhotoTags.Any(pt => pt.TagId == tagFilter.Value));
            var total = await query.CountAsync();
            var photos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.Tags = new SelectList(await _context.Tag.ToListAsync(), "Id", "Name");
            ViewBag.SearchText = searchText;
            ViewBag.TagFilter = tagFilter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            return View(photos.Select(p => new PhotoViewModel
            {
                Id = p.Id, Name = p.Name, Description = p.Description, ImageUrl = p.ImageUrl,
                Price = p.Price, PhotographerName = p.Photographer?.Name,
                TagNames = p.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
            }).ToList());
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var photo = await _context.Photo.Include(p => p.Photographer).Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ViewBag.AlreadyDownloaded = await _context.Download.AnyAsync(d => d.PhotoId == id && d.AppUserId == userId);
            return View(new PhotoViewModel
            {
                Id = photo.Id, Name = photo.Name, Description = photo.Description, ImageUrl = photo.ImageUrl,
                Price = photo.Price, CreatedAt = photo.CreatedAt, PhotographerName = photo.Photographer?.Name,
                TagNames = photo.PhotoTags.Select(pt => pt.Tag!.Name).ToList()
            });
        }

        [HttpPost][Authorize]
        public async Task<IActionResult> Download(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await _context.Download.AnyAsync(d => d.PhotoId == id && d.AppUserId == userId))
            {
                _context.Download.Add(new Download { PhotoId = id, AppUserId = userId, DownloadedAt = DateTime.UtcNow });
                await _context.SaveChangesAsync();
            }
            TempData["DownloadSuccess"] = "Fotografija je uspješno preuzeta!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserDownloads()
        {
            var users = await _context.AppUser.Where(u => u.Role == "User").Include(u => u.Downloads).ThenInclude(d => d.Photo).ToListAsync();
            return View(users);
        }
    }
}
