using Core.Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Core
{
    public class TourGuideService : ITourGuideService
    {
        private readonly TahiraTravelsDbContext _context;

        public TourGuideService(TahiraTravelsDbContext context)
        {
            _context = context;
        }

        public async Task<TourGuide?> GetGuideForTourAsync(int tourId)
        {
            return await _context.TourGuides
                .FirstOrDefaultAsync(g => g.TourId == tourId);
        }

        public async Task<TourGuide?> GetGuideByIdAsync(int id)
        {
            return await _context.TourGuides.FindAsync(id);
        }
        public async Task AddGuideAsync(TourGuide guide)
        {
            _context.TourGuides.Add(guide);
            await _context.SaveChangesAsync();
        }
    }
}
