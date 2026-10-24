namespace WebAPI.DTOs
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TagCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
