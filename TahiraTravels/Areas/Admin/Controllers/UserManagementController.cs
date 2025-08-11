using Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using ViewModels.ViewModels.Admin.UserManagement;

namespace TahiraTravels.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : BaseAdminController
    {
        private readonly IUserService userService;

        public UserManagementController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<UserManagementIndexViewModel> allUsers = await this.userService
                .GetUserManagementBoardDataAsync(this.GetUserId()!);

            return View(allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleSelectionInputModel inputModel)
        {
            try
            {
                await this.userService
                    .AssignUserToRoleAsync(inputModel);
                //TempData[SuccessMessageKey] = "User assigned to role successfully!";

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                //TempData[ErrorMessageKey] = e.Message;

                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
