using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly LogService _logService;

        public AuthController(AppDbContext context, JwtService jwtService, LogService logService)
        {
            _context = context; _jwtService = jwtService; _logService = logService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.AppUser.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest(new { message = "Korisnik s tim korisničkim imenom ili emailom već postoji." });
            var user = new AppUser
            {
                Name = dto.Username, Username = dto.Username, Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName, LastName = dto.LastName, Role = "User"
            };
            _context.AppUser.Add(user);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Novi korisnik registriran: {user.Username}");
            return Ok(new { message = "Registracija uspješna." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.AppUser.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                await _logService.LogAsync($"Neuspješna prijava za: {dto.Username}", "Warning");
                return Unauthorized(new { message = "Neispravno korisničko ime ili lozinka." });
            }
            var token = _jwtService.GenerateToken(user);
            await _logService.LogAsync($"Korisnik {user.Username} se prijavio.");
            return Ok(new TokenResponseDto { Token = token, Username = user.Username, Role = user.Role });
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _context.AppUser.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return Unauthorized(new { message = "Neispravna stara lozinka." });
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            await _logService.LogAsync($"Korisnik {user.Username} je promijenio lozinku.");
            return Ok(new { message = "Lozinka uspješno promijenjena." });
        }
    }
}
