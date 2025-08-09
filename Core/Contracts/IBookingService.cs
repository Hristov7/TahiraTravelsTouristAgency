using Models;

namespace Core.Contracts
{
    public interface IBookingService
    {
        Task CreateBookingAsync(Booking booking, string userId);
        Task<List<Booking>> GetUserBookingsAsync(string userId);
        Task<bool> DeleteBookingAsync(int bookingId, string userId);
    }
}
