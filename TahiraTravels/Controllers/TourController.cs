using Core;
using Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewModels.ViewModels;

namespace TahiraTravels.Controllers
{
    public class TourController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ITourService _tourService;
        private readonly IReviewService _reviewService;
        private readonly ITourGuideService _tourGuideService;
        public TourController(ICategoryService _categoryService, ITourService _tourService, IReviewService reviewService, ITourGuideService tourGuideService) 
        {
            this._categoryService = _categoryService;
            this._tourService = _tourService;
            this._reviewService = reviewService;
            this._tourGuideService = tourGuideService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _tourService.GetAllToursAsync(userId);
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(t => t.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _tourService.GetTourDetailsAsync(id, userId);

                // Add reviews
                model.Reviews = await _reviewService.GetReviewsForTourAsync(id);

                // Check if user can review
                model.CanReview = userId != null &&
                                  await _reviewService.HasUserBookedTourAsync(id, userId);

                // Load tour guide
                var guide = await _tourGuideService.GetGuideForTourAsync(id);
                model.Guide = guide;

                return View(model);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddReview(int tourId)
        {
            string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !await _reviewService.HasUserBookedTourAsync(tourId, userId))
            {
                return Forbid();
            }
            ViewBag.TourId = tourId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddReview(int tourId, string comment)
        {
            string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !await _reviewService.HasUserBookedTourAsync(tourId, userId))
            {
                return Forbid();
            }

            await _reviewService.AddReviewAsync(tourId, userId, comment);
            return RedirectToAction("Details", new { id = tourId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var model = new TourCreateInputModel
            {
                Categories = categories
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(TourCreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _tourService.CreateTourAsync(model, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _tourService.GetTourForEditAsync(id, userId);
                return View(model);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(TourEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _tourService.EditTourAsync(model, userId);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _tourService.GetTourForDeleteAsync(id, userId);
                return View(model);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _tourService.DeleteTourAsync(id, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Save(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _tourService.SaveTourAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _tourService.RemoveTourFromFavoritesAsync(id, userId);
            return RedirectToAction(nameof(Favorites));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _tourService.GetFavoriteToursAsync(userId);
            return View(model);
        }
    }
}
