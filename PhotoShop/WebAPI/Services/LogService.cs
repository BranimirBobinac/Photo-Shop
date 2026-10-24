using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;
        public LogService(AppDbContext context) { _context = context; }

        public async Task LogAsync(string message, string level = "Info")
        {
            _context.Log.Add(new Log { Message = message, Level = level, Timestamp = DateTime.UtcNow });
            await _context.SaveChangesAsync();
        }
    }
}
