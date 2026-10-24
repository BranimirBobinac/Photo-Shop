using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Photographer
    {
        public int Id { get; set; }
        [Required][MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Bio { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}
