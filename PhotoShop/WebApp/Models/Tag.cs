using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Naziv oznake je obavezan.")][MaxLength(100)][Display(Name = "Oznaka")]
        public string Name { get; set; } = string.Empty;
        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
    }
}
