using Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels.ViewModels;

namespace TahiraTravels.Controllers
{
    public class TourGuideController : Controller
    {
        private readonly ITourGuideService _tourGuideService;

        public TourGuideController(ITourGuideService tourGuideService)
        {
            _tourGuideService = tourGuideService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var guide = await _tourGuideService.GetGuideByIdAsync(id);
            if (guide == null) return NotFound();
            return View(guide);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(int tourId)
        {
            var model = new TourGuideViewModel
            {
                TourId = tourId
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(TourGuideViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new TourGuide
            {
                TourId = model.TourId,
                Name = model.Name,
                Age = model.Age,
                Location = model.Location,
                Languages = model.Languages,
                ExperienceYears = model.ExperienceYears
            };

            await _tourGuideService.AddGuideAsync(entity);

            return RedirectToAction("Details", "Tour", new { id = model.TourId });
        }
    }
}
