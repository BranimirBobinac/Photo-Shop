namespace WebAPI.DTOs
{
    public class PhotographerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Email { get; set; }
    }

    public class PhotographerCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Email { get; set; }
    }
}
