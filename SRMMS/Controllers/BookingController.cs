using Microsoft.AspNetCore.Mvc;

namespace SRMMS.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
