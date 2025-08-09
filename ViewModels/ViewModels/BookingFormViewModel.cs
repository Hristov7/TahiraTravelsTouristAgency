using System.ComponentModel.DataAnnotations;

namespace ViewModels.ViewModels
{
    public class BookingFormViewModel
    {
        public int TourId { get; set; }
        [Required]
        public int NumberOfPeople { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
    }

}
