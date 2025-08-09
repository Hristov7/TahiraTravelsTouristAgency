using Core.Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Core
{
    public class BookingService : IBookingService
    {
        private readonly TahiraTravelsDbContext _context;

        public BookingService(TahiraTravelsDbContext context)
        {
            _context = context;
        }

        public async Task CreateBookingAsync(Booking booking, string userId)
        {
            booking.UserId = userId;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBookingAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings
           .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
            {
                return false;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
    }
}
