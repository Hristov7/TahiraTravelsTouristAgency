using Microsoft.AspNetCore.Mvc;

namespace TahiraTravels.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
