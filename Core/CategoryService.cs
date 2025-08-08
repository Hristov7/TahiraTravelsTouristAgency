using Core.Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using ViewModels.ViewModels;

namespace Core
{
    public class CategoryService : ICategoryService
    {
        private readonly TahiraTravelsDbContext dbContext;

        public CategoryService(TahiraTravelsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            return await dbContext.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
