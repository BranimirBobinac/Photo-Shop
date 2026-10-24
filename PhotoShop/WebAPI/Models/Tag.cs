using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required][MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
    }
}
