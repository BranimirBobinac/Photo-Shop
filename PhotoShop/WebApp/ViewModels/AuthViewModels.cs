using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")][Display(Name = "Korisničko ime")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lozinka je obavezna.")][DataType(DataType.Password)][Display(Name = "Lozinka")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")][Display(Name = "Korisničko ime")]
        public string Username { get; set; } = string.Empty;
        [Required][EmailAddress(ErrorMessage = "Unesite ispravnu e-mail adresu.")][Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Ime")] public string? FirstName { get; set; }
        [Display(Name = "Prezime")] public string? LastName { get; set; }
        [Required][DataType(DataType.Password)][Display(Name = "Lozinka")]
        public string Password { get; set; } = string.Empty;
        [Required][DataType(DataType.Password)][Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")][Display(Name = "Ponovi lozinku")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ProfileViewModel
    {
        public string Username { get; set; } = string.Empty;
        [Required][EmailAddress][Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Ime")] public string? FirstName { get; set; }
        [Display(Name = "Prezime")] public string? LastName { get; set; }
        [Phone][Display(Name = "Telefon")] public string? Phone { get; set; }
        [DataType(DataType.Password)][Display(Name = "Nova lozinka")]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)][Compare("NewPassword", ErrorMessage = "Lozinke se ne podudaraju.")][Display(Name = "Ponovi lozinku")]
        public string? ConfirmPassword { get; set; }
    }
}
