using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Naziv je obavezan.")][MaxLength(200)][Display(Name = "Naziv")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Opis")][MaxLength(1000)]
        public string? Description { get; set; }
        [Required(ErrorMessage = "URL slike je obavezan.")][Url(ErrorMessage = "Unesite ispravan URL.")][Display(Name = "URL slike")]
        public string ImageUrl { get; set; } = string.Empty;
        [Required][Column(TypeName = "decimal(10,2)")][Display(Name = "Cijena (EUR)")][Range(0, 99999)]
        public decimal Price { get; set; }
        [Display(Name = "Datum objave")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Fotograf je obavezan.")][Display(Name = "Fotograf")]
        public int PhotographerId { get; set; }
        public Photographer? Photographer { get; set; }
        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
        public ICollection<Download> Downloads { get; set; } = new List<Download>();
    }
}
