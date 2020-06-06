using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}