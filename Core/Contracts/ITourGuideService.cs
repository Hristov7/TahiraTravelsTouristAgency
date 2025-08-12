using Models;

namespace Core.Contracts
{
    public interface ITourGuideService
    {
        Task<TourGuide?> GetGuideForTourAsync(int tourId);
        Task<TourGuide?> GetGuideByIdAsync(int id);
        Task AddGuideAsync(TourGuide guide);
    }
}
