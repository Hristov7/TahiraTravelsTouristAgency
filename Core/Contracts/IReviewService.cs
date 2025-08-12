using ViewModels.ViewModels;

namespace Core.Contracts
{
    public interface IReviewService
    {
        Task<List<ReviewViewModel>> GetReviewsForTourAsync(int tourId);
        Task AddReviewAsync(int tourId, string userId, string comment);
        Task<bool> HasUserBookedTourAsync(int tourId, string userId);
    }
}
