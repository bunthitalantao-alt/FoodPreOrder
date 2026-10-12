using Microsoft.AspNetCore.Mvc;

namespace FoodPreOrder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Customer");
        }
    }
}
