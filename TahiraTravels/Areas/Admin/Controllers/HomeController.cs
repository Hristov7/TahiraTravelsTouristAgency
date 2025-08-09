using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TahiraTravels.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
