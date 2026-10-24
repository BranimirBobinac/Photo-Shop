namespace WebAPI.Models
{
    public class Download
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }
        public int AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    }
}
