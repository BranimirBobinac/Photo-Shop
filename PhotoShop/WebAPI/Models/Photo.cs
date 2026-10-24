using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required][MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int PhotographerId { get; set; }
        public Photographer? Photographer { get; set; }
        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
        public ICollection<Download> Downloads { get; set; } = new List<Download>();
    }
}
