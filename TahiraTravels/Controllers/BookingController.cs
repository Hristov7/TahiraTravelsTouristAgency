using Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels.ViewModels;

namespace TahiraTravels.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(IBookingService bookingService, UserManager<IdentityUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Book(int tourId)
        {
            var booking = new Booking
            {
                TourId = tourId
            };

            return View("BookingForm", booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(BookingFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var booking = new Booking
            {
                TourId = model.TourId,
                NumberOfPeople = model.NumberOfPeople,
                BookingDate = model.BookingDate,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingService.CreateBookingAsync(booking,user.Id);
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);

            bool result = await _bookingService.DeleteBookingAsync(id, user.Id);

            if (!result)
                return View("ErrorPage500");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _bookingService.GetUserBookingsAsync(user.Id);

            return View(bookings);
        }
    }

}
