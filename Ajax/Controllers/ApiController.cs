using Ajax.Models;
using Ajax.Models.ViewModels;
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
            //等五秒
            System.Threading.Thread.Sleep(10000);

            //return View();
            //return Content("回傳純文字","text/plain",System.Text.Encoding.UTF8);
            //return Content("<h2>Hello Content, 您好</h2>", "text/html", System.Text.Encoding.UTF8);
            return Content("Hello Content, 您好!", "text/plain", System.Text.Encoding.UTF8);
        }
        //Get 方法

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _dbContext.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                return File(img, "image/jpeg");
            }
            return NotFound(); //404
        }

        public IActionResult Register(UserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                dto.Name = "Gust";
            }
            //return Content($"Hello {dto.Name}, {dto.Age}歲了，電子信箱是 {dto.Email}");
            return Content($"{dto.Avatar?.FileName}-{dto.Avatar?.Length}-{dto.Avatar?.ContentType}");
        }

        public IActionResult Cities()
        {
            var citiers = _dbContext.Addresses.Select(a => a.City).Distinct();
            //return View();
            return Json(citiers);
        }
        public IActionResult Districts(string city)
        {
            var districts = _dbContext.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        public IActionResult Rouds(string district)
        {
            var rounds = _dbContext.Addresses.Where(a => a.SiteId == district).Select(a => a.Road).Distinct();
            return Json(rounds);
        }
        public IActionResult CheckAccount(string name)
        {
            var result = _dbContext.Members.Where(m => m.Name.Contains(name)).Any();
            if (result == false)
            {
                return Content("帳號可以使用", "text/plain", System.Text.Encoding.UTF8);
            }
            return Content("帳號已存在", "text/plain", System.Text.Encoding.UTF8);
        }
    }
}
