using Core.Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels.ViewModels;

namespace Core
{
    public class TourService : ITourService
    {
        private readonly TahiraTravelsDbContext dbContext;
        public TourService(TahiraTravelsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<TourIndexViewModel>> GetAllToursAsync(string userId)
        {
            return await dbContext.Destinations
                .Where(r => !r.IsDeleted)
                .Select(r => new TourIndexViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Category = r.Category.Name,
                    ImageUrl = r.ImageUrl,
                    SavedCount = r.UsersDestinations.Count,
                    IsAuthor = r.AuthorId == userId,
                    IsSaved = r.UsersDestinations.Any(ur => ur.UserId == userId)
                })
                .ToListAsync();
        }

        public async Task<TourDetailsViewModel> GetTourDetailsAsync(int id, string userId)
        {
            var tour = await dbContext.Destinations
                .Include(r => r.Category)
                .Include(r => r.Author)
                .Include(r => r.UsersDestinations)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (tour == null)
            {
                throw new ArgumentException("Destination not found!");
            }

            return new TourDetailsViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                ImageUrl = tour.ImageUrl,
                Category = tour.Category.Name,
                CreatedOn = tour.CreatedOn.ToString("dd-MM-yyyy"),
                Author = tour.Author.UserName,
                AuthorId = tour.AuthorId,
                IsAuthor = tour.AuthorId == userId,
                IsSaved = tour.UsersDestinations.Any(ur => ur.UserId == userId)
            };
        }

        public async Task CreateTourAsync(TourCreateInputModel model, string userId)
        {
            var tour = new Destination
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                AuthorId = userId,
                CreatedOn = DateTime.Parse(model.CreatedOn)
            };

            await dbContext.Destinations.AddAsync(tour);
            await dbContext.SaveChangesAsync();
        }

        public async Task<TourEditInputModel> GetTourForEditAsync(int id, string userId)
        {
            var tour = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (tour == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to edit it!");
            }

            var categories = await dbContext.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return new TourEditInputModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                ImageUrl = tour.ImageUrl,
                CategoryId = tour.CategoryId,
                CreatedOn = tour.CreatedOn.ToString("yyyy-MM-dd"),
                Categories = categories
            };
        }

        public async Task EditTourAsync(TourEditInputModel model, string userId)
        {
            var tour = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == model.Id && r.AuthorId == userId && !r.IsDeleted);

            if (tour == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to edit it!");
            }

            tour.Name = model.Name;
            tour.Description = model.Description;
            tour.ImageUrl = model.ImageUrl;
            tour.CategoryId = model.CategoryId;
            tour.CreatedOn = DateTime.Parse(model.CreatedOn);

            await dbContext.SaveChangesAsync();
        }

        public async Task<TourDeleteViewModel> GetTourForDeleteAsync(int id, string userId)
        {
            var tour = await dbContext.Destinations
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (tour == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to delete it!");
            }

            return new TourDeleteViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Author = tour.Author.UserName,
                AuthorId = tour.AuthorId
            };
        }
        public async Task DeleteTourAsync(int id, string userId)
        {
            var tour = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (tour == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to delete it!");
            }

            tour.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveTourAsync(int tourId, string userId)
        {
            var alreadySaved = await dbContext.UsersDestinations
                .AnyAsync(ur => ur.DestinationId == tourId && ur.UserId == userId);

            if (alreadySaved)
            {
                return;
            }

            var userDestination = new UserDestination
            {
                DestinationId = tourId,
                UserId = userId
            };

            await dbContext.UsersDestinations.AddAsync(userDestination);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTourFromFavoritesAsync(int tourId, string userId)
        {
            var userDestination = await dbContext.UsersDestinations
                .FirstOrDefaultAsync(ur => ur.DestinationId == tourId && ur.UserId == userId);

            if (userDestination != null)
            {
                dbContext.UsersDestinations.Remove(userDestination);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TourFavoriteViewModel>> GetFavoriteToursAsync(string userId)
        {
            return await dbContext.UsersDestinations
               .Where(ur => ur.UserId == userId)
               .Select(ud => new TourFavoriteViewModel
               {
                   Id = ud.Destination.Id,
                   Name = ud.Destination.Name,
                   Category = ud.Destination.Category.Name,
                   ImageUrl = ud.Destination.ImageUrl
               })
               .ToListAsync();
        }

        public async Task<bool> IsTourSavedByUserAsync(int tourId, string userId)
        {
            return await dbContext.UsersDestinations
                .AnyAsync(ud => ud.DestinationId == tourId && ud.UserId == userId);
        }
    }
}
