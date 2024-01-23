using Microsoft.AspNetCore.Mvc;

namespace Ajax.Controllers
{
    public class HomeworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
