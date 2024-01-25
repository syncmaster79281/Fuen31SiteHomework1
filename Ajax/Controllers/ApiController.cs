using Ajax.Models;
using Ajax.Models.Dtos;
using Ajax.Models.Utilitys;
using Microsoft.AspNetCore.Mvc;

namespace Ajax.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dbContext;
        //取得實際路徑
        private readonly IWebHostEnvironment _host;

        public ApiController(MyDBContext dbContext, IWebHostEnvironment host)
        {
            //DI注入
            _dbContext = dbContext;
            _host = host;
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

        //加入[FromBody] 才能讀出中文
        public IActionResult Spots([FromBody] SearchDto _json)
        {
            //根據分類編號讀取景點資料 SpotImagesSpots 為SQL 的View 資料表
            var spots = _json.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == _json.categoryId);

            //根據分類編號 內的關鍵字搜尋 如果分類編號為0搜尋整個
            if (!string.IsNullOrEmpty(_json.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_json.keyword) || s.SpotDescription.Contains(_json.keyword));
            }

            //排序功能
            switch (_json.sortBy)
            {
                case "spotTitle":
                    spots = _json.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _json.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = _json.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //分頁功能
            //總共有多少筆資料
            int TotalCount = spots.Count();
            //設定每頁顯示多少筆資料
            int pageSize = _json.pageSize ?? 9;       // ??  如果_json.pageSize 為null則設定為9 否則為_json.pageSize
            //計算總共有幾頁
            int TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);
            //目前要顯示第幾頁
            int page = _json.page ?? 1; //第幾頁

            //限制最大頁數
            page = page > TotalPages ? TotalPages : page;

            //取出分頁資料
            spots = spots.Skip((int)((page - 1) * pageSize)).Take(pageSize); //跳過幾筆拿幾筆

            SpotsPagingDto spotJson = new SpotsPagingDto
            {

                TotalPages = TotalPages,
                SpotsResult = spots.ToList(),

            };

            return Json(spotJson);
        }

        public IActionResult SpotTitle(string keyword)
        {
            var spots = _dbContext.Spots.Where(s => s.SpotTitle.Contains(keyword))
               .Select(s => s.SpotTitle).Take(8);
            return Json(spots);
        }

        public IActionResult SpotCategories()
        {
            var categories = _dbContext.Categories.Select(c => new SpotCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
            return Json(categories);
        }


        [HttpPost]
        public IActionResult Register(Member member, IFormFile Avatar)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                member.Name = "Gust";
            }

            //todo: 判斷檔案是否存在
            //todo: 限制檔案類型 .jpg .png
            //Todo: 限制檔案上傳大小

            string fileName = "empty.jpg";

            if (Avatar != null)
            {
                fileName = Avatar.FileName;
            }

            //_host.WebRootPath 取得wwwroot的實際路徑 /uploads/fileName
            var uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);

            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                Avatar?.CopyTo(fileStream);
            }

            //轉成二進位檔案
            byte[]? imgByte;
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }

            member.FileName = fileName;
            member.FileData = imgByte;

            var salt = HashUtility.GenerateSalt(16);
            member.Salt = Convert.ToBase64String(salt);
            member.Password = HashUtility.ToSHA256(member.Password, member.Salt);

            //新增資料庫
            _dbContext.Members.Add(member);
            _dbContext.SaveChanges();

            return Content("新增成功", "text/plain", System.Text.Encoding.UTF8);
        }

        //public IActionResult Register(UserDto dto)
        //{
        //    if (string.IsNullOrEmpty(dto.Name))
        //    {
        //        dto.Name = "Gust";
        //    }

        //    //todo: 判斷檔案是否存在
        //    //todo: 限制檔案類型 .jpg .png
        //    //Todo: 限制檔案上傳大小

        //    string fileName = "empty.jpg";

        //    if (dto.Avatar != null)
        //    {
        //        fileName = dto.Avatar.FileName;
        //    }

        //    //_host.WebRootPath 取得wwwroot的實際路徑 /uploads/fileName
        //    var uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);

        //    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
        //    {
        //        dto.Avatar?.CopyTo(fileStream);
        //    }

        //    //轉成二進位檔案
        //    byte[]? imgByte = null;
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        dto.Avatar?.CopyTo(memoryStream);
        //        imgByte = memoryStream.ToArray();
        //    }

        //    //return Content($"Hello {dto.Name}, {dto.Age}歲了，電子信箱是 {dto.Email}");
        //    return Content($"{dto.Avatar?.FileName}-{dto.Avatar?.Length}-{dto.Avatar?.ContentType}");
        //}

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
