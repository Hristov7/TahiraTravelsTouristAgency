using Microsoft.AspNetCore.Identity;
using Models.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        public int TourId { get; set; }
        public Destination Tour { get; set; } = null!;

        [Required]
        [Range(1, 20, ErrorMessage = "Number of people must be between 1 and 20")]
        public int NumberOfPeople { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Booking date must be in the future")]
        public DateTime BookingDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
