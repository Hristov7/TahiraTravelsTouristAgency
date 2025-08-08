namespace ViewModels.ViewModels
{
    public class TourDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = null!;
        public string CreatedOn { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public bool IsAuthor { get; set; }
        public bool IsSaved { get; set; }
    }
}
