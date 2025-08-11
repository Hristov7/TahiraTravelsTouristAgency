using System.ComponentModel.DataAnnotations;

namespace ViewModels.ViewModels.Admin.UserManagement
{
    public class RoleSelectionInputModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
