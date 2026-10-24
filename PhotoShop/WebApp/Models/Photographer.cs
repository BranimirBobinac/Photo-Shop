using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Photographer
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ime je obavezno.")][MaxLength(200)][Display(Name = "Ime fotografa")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Biografija")][MaxLength(500)]
        public string? Bio { get; set; }
        [EmailAddress(ErrorMessage = "Unesite ispravnu e-mail adresu.")][Display(Name = "E-mail")]
        public string? Email { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}
