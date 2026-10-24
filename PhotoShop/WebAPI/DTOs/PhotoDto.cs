namespace WebAPI.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PhotographerId { get; set; }
        public string? PhotographerName { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class PhotoCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PhotographerId { get; set; }
        public List<int> TagIds { get; set; } = new();
    }

    public class PhotoUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PhotographerId { get; set; }
        public List<int> TagIds { get; set; } = new();
    }
}
