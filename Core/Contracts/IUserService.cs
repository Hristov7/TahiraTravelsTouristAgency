using ViewModels.ViewModels.Admin.UserManagement;

namespace Core.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserManagementIndexViewModel>> GetUserManagementBoardDataAsync(string userId);

        Task<bool> AssignUserToRoleAsync(RoleSelectionInputModel inputModel);
    }
}
