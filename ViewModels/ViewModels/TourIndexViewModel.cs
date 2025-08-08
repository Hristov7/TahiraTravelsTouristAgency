namespace ViewModels.ViewModels
{
    public class TourIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int SavedCount { get; set; }
        public bool IsAuthor { get; set; }
        public bool IsSaved { get; set; }
    }
}
