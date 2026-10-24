using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [Required][MaxLength(100)][Display(Name = "Korisničko ime")]
        public string Name { get; set; } = string.Empty;
        [Required][MaxLength(100)][Display(Name = "Korisničko ime")]
        public string Username { get; set; } = string.Empty;
        [Required][EmailAddress][Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        [Display(Name = "Ime")] public string? FirstName { get; set; }
        [Display(Name = "Prezime")] public string? LastName { get; set; }
        [Phone][Display(Name = "Telefon")] public string? Phone { get; set; }
        public ICollection<Download> Downloads { get; set; } = new List<Download>();
    }
}
