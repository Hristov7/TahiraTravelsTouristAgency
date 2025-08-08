namespace ViewModels.ViewModels
{
    public class TourFavoriteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
