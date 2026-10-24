using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class PhotoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Naziv je obavezan.")][Display(Name = "Naziv")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Opis")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "URL slike je obavezan.")][Url(ErrorMessage = "Unesite ispravan URL.")][Display(Name = "URL slike")]
        public string ImageUrl { get; set; } = string.Empty;
        [Required][Display(Name = "Cijena (EUR)")][Range(0, 99999)]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Odaberite fotografa.")][Display(Name = "Fotograf")]
        public int PhotographerId { get; set; }
        public List<int> SelectedTagIds { get; set; } = new();
        public SelectList? Photographers { get; set; }
        public SelectList? Tags { get; set; }
        public List<string> TagNames { get; set; } = new();
        public string? PhotographerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PhotoListViewModel
    {
        public List<PhotoViewModel> Photos { get; set; } = new();
        public string? SearchText { get; set; }
        public int? PhotographerFilter { get; set; }
        public SelectList? Photographers { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
