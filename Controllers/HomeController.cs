using BLL.Servises.Abstractions;
using Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartCourses.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCourses.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IService<User> _userService;
        private readonly IService<Course> _courseService;

        public HomeController(ILogger<CourseController> logger, IService<Course> courseService, IService<User> userService)
        {
            _courseService = courseService;
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Contact()
        {
            _logger.LogInformation("Home contact -> Ok");
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeModel();
            var courseResponce = await _courseService.GetAll();
            var userResponce = await _userService.GetAll();

            model.TopCourses = courseResponce.Data.OrderByDescending(c => c.Videos.Count + c.Books.Count + c.Articles.Count).Take(3).ToList();
            model.TopUsers = userResponce.Data.OrderByDescending(u => u.Profile.Courses.Count).Take(6).ToList();

            _logger.LogInformation("Home index -> Ok");

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
