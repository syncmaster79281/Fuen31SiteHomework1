using Ajax.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ajax.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dbContext;

        public ApiController(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cities()
        {
            var citiers = _dbContext.Addresses.Select(a => a.City).Distinct();
            //return View();
            return Json(citiers);
        }
    }
}
