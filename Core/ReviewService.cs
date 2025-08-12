using Core.Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels.ViewModels;

namespace Core
{
    public class ReviewService : IReviewService
    {
        private readonly TahiraTravelsDbContext _context;

        public ReviewService(TahiraTravelsDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewViewModel>> GetReviewsForTourAsync(int tourId)
        {
            return await _context.Reviews
                .Where(r => r.TourId == tourId)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserName = r.User.UserName!,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddReviewAsync(int tourId, string userId, string comment)
        {
            var review = new Review
            {
                TourId = tourId,
                UserId = userId,
                Comment = comment
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserBookedTourAsync(int tourId, string userId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.TourId == tourId && b.UserId == userId);
        }
    }
}
