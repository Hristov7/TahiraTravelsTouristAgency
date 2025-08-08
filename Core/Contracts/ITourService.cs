using ViewModels.ViewModels;

namespace Core.Contracts
{
    public interface ITourService
    {
        Task<IEnumerable<TourIndexViewModel>> GetAllToursAsync(string userId);
        Task<TourDetailsViewModel> GetTourDetailsAsync(int id, string userId);
        Task CreateTourAsync(TourCreateInputModel model, string userId);
        Task<TourEditInputModel> GetTourForEditAsync(int id, string userId);
        Task EditTourAsync(TourEditInputModel model, string userId);
        Task<TourDeleteViewModel> GetTourForDeleteAsync(int id, string userId);
        Task DeleteTourAsync(int id, string userId);
        Task SaveTourAsync(int TourId, string userId);
        Task RemoveTourFromFavoritesAsync(int TourId, string userId);
        Task<IEnumerable<TourFavoriteViewModel>> GetFavoriteToursAsync(string userId);
        Task<bool> IsTourSavedByUserAsync(int TourId, string userId);
    }
}
