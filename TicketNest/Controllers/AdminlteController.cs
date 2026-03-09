using Microsoft.AspNetCore.Mvc;

namespace TicketNest.Controllers
{
    public class AdminlteController : Controller
    {
        public IActionResult Blank()
        {
            return View();
        }
    }
}
