using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [Required][MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required][MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required][EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public ICollection<Download> Downloads { get; set; } = new List<Download>();
    }
}
