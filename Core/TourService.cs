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
            var recipe = await dbContext.Destinations
                .Include(r => r.Category)
                .Include(r => r.Author)
                .Include(r => r.UsersDestinations)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null)
            {
                throw new ArgumentException("Destination not found!");
            }

            return new TourDetailsViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                Category = recipe.Category.Name,
                CreatedOn = recipe.CreatedOn.ToString("dd-MM-yyyy"),
                Author = recipe.Author.UserName,
                AuthorId = recipe.AuthorId,
                IsAuthor = recipe.AuthorId == userId,
                IsSaved = recipe.UsersDestinations.Any(ur => ur.UserId == userId)
            };
        }

        public async Task CreateTourAsync(TourCreateInputModel model, string userId)
        {
            var recipe = new Destination
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                AuthorId = userId,
                CreatedOn = DateTime.Parse(model.CreatedOn)
            };

            await dbContext.Destinations.AddAsync(recipe);
            await dbContext.SaveChangesAsync();
        }

        public async Task<TourEditInputModel> GetTourForEditAsync(int id, string userId)
        {
            var recipe = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (recipe == null)
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
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                CategoryId = recipe.CategoryId,
                CreatedOn = recipe.CreatedOn.ToString("yyyy-MM-dd"),
                Categories = categories
            };
        }

        public async Task EditTourAsync(TourEditInputModel model, string userId)
        {
            var recipe = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == model.Id && r.AuthorId == userId && !r.IsDeleted);

            if (recipe == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to edit it!");
            }

            recipe.Name = model.Name;
            recipe.Description = model.Description;
            recipe.ImageUrl = model.ImageUrl;
            recipe.CategoryId = model.CategoryId;
            recipe.CreatedOn = DateTime.Parse(model.CreatedOn);

            await dbContext.SaveChangesAsync();
        }

        public async Task<TourDeleteViewModel> GetTourForDeleteAsync(int id, string userId)
        {
            var recipe = await dbContext.Destinations
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (recipe == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to delete it!");
            }

            return new TourDeleteViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Author = recipe.Author.UserName,
                AuthorId = recipe.AuthorId
            };
        }
        public async Task DeleteTourAsync(int id, string userId)
        {
            var recipe = await dbContext.Destinations
                .FirstOrDefaultAsync(r => r.Id == id && r.AuthorId == userId && !r.IsDeleted);

            if (recipe == null)
            {
                throw new ArgumentException("Destination not found or you don't have permission to delete it!");
            }

            recipe.IsDeleted = true;
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

            var userRecipe = new UserDestination
            {
                DestinationId = tourId,
                UserId = userId
            };

            await dbContext.UsersDestinations.AddAsync(userRecipe);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTourFromFavoritesAsync(int tourId, string userId)
        {
            var userRecipe = await dbContext.UsersDestinations
                .FirstOrDefaultAsync(ur => ur.DestinationId == tourId && ur.UserId == userId);

            if (userRecipe != null)
            {
                dbContext.UsersDestinations.Remove(userRecipe);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TourFavoriteViewModel>> GetFavoriteToursAsync(string userId)
        {
            return await dbContext.UsersDestinations
               .Where(ur => ur.UserId == userId)
               .Select(ur => new TourFavoriteViewModel
               {
                   Id = ur.Destination.Id,
                   Name = ur.Destination.Name,
                   Category = ur.Destination.Category.Name,
                   ImageUrl = ur.Destination.ImageUrl
               })
               .ToListAsync();
        }

        public async Task<bool> IsTourSavedByUserAsync(int tourId, string userId)
        {
            return await dbContext.UsersDestinations
                .AnyAsync(ur => ur.DestinationId == tourId && ur.UserId == userId);
        }
    }
}
