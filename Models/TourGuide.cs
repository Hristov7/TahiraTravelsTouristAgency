using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TourGuide
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Range(18, 100)]
        public int Age { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public string Languages { get; set; } = null!;

        [Required]
        public int ExperienceYears { get; set; }

        [ForeignKey(nameof(Tour))]
        public int TourId { get; set; }
        public Destination Tour { get; set; } = null!;
    }
}
