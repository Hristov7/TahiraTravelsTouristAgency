namespace ViewModels.ViewModels
{
    public class TourGuideViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Location { get; set; } = null!;
        public string Languages { get; set; } = null!;
        public int ExperienceYears { get; set; }
    }
}
