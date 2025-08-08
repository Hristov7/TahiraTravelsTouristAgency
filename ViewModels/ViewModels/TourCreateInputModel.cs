using System.ComponentModel.DataAnnotations;

namespace ViewModels.ViewModels
{
    public class TourCreateInputModel
    {
        [Required]
        [StringLength(80, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        [Required]
        public string CreatedOn { get; set; } = null!;
    }
}
