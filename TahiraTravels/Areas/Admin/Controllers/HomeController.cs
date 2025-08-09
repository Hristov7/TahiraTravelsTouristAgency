using Microsoft.AspNetCore.Mvc;

namespace TahiraTravels.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
