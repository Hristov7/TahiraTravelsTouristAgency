namespace ViewModels.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
