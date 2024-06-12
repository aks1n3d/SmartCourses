using BLL.Servises.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Core.Entity;
using SmartCourses.Models;

namespace SmartCourses.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IAccountService _accountService;
        private readonly IService<User> _userService;
        private readonly IService<Course> _courseService;

        public AccountController(ILogger<CourseController> logger, IService<Course> courseService, IAccountService accountService, IService<User> userService)
        {
            _courseService = courseService;
            _logger = logger;
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> ViewAll(AccountsModel model)
        {
            var searchString = model.Search;
            var response = await _userService.GetAll();
            var users = response.Data;

            if (searchString is not null)
            {
                users = users.Where(u => 
                u.Profile.FirstName.ToLower().Contains(searchString.ToLower()) |
                u.Profile.SecondName.ToLower().Contains(searchString.ToLower()) |
                u.Profile.AboutMe.ToLower().Contains(searchString.ToLower()) | 
                u.Profile.Age.ToString().Contains(searchString.ToLower()) |
                u.Profile.Courses.Any(c => c.Name.ToLower().Contains(searchString.ToLower()) | c.Description.ToLower().Contains(searchString.ToLower())) |
                u.Profile.Skills.Any(s => s.Name.ToLower().Contains(searchString.ToLower()) | s.Description.ToLower().Contains(searchString.ToLower())))
                .ToList();
            }

            List<int> ids = new();

            foreach (var user in users)
            {
                ids.Add(user.Id);
            }

            var stringIds = string.Join(";", ids);

            if (stringIds.Length == 0)
            {
                stringIds = "empty";
            }

            return RedirectToAction("ViewAll", "Account", new { ids = stringIds });
        }

        public async Task<IActionResult> ViewAll(string ids = null)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _userService.GetAll();
            var users = response.Data;

            if (ids is not null)
            {
                if (ids != "empty")
                {
                    var idsList = ids.Split(";");
                    var sortedUsers = users.Where(c => idsList.Contains(c.Id.ToString())).ToList();
                    return View(new AccountsModel() { Data = sortedUsers });
                }
                else
                {
                    return View(new AccountsModel() { Data = new List<User>() });
                }
            }

            _logger.LogInformation("Get All Users -> Ok");

            return View(new AccountsModel() { Data = users });
        }

        public async Task<IActionResult> Edit()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var response = await _userService.Get(id);

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,Login,Password,Profile")] User user)
        {
            if (ModelState.IsValid)
            {
                var resposnce = await _userService.Update(user);

                if (resposnce.StatusCode == Core.Enums.StatusCode.Ok)
                {
                    _logger.LogInformation("AddOrEdit User -> Ok");
                    return RedirectToAction("Profile"); 
                }

                _logger.LogError("AddOrEdit User -> Error");

            }
            _logger.LogError("AddOrEdit User -> Error");

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id = 0)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == 0)
            {
                id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            }

            var response = await _userService.Get(id);
            var responceCourses = await _courseService.GetAll();
            var authorCourses = responceCourses.Data.Where(c => c.AuthorId == response.Data.Id).ToList();
            _logger.LogInformation("View User -> Ok");

            var model = new ProfileViewModel() { Data = response.Data, AuthorCourses = authorCourses };

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel model = new();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var responce = await _accountService.Register(model);


                if(responce.StatusCode == Core.Enums.StatusCode.Ok)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(responce.Data));

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", responce.Info);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var responce = await _accountService.Login(model);


                if (responce.StatusCode == Core.Enums.StatusCode.Ok)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(responce.Data));

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
