using BLL.Servises.Abstractions;
using Core.Entity;
using Core.Entity.Abstractions;
using Core.Static;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SmartCourses.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartCourses.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IService<Course> _courseService;
        private readonly IService<User> _userService;
        private readonly Dictionary<int, Type> _materialTypeDic = new Dictionary<int, Type>()
        {
            {1, typeof(VideoMaterial) },
            {2, typeof(BookMaterial) },
            {3, typeof(ArticleMaterial) },
        };


        public CourseController(ILogger<CourseController> logger, IService<Course> courseService, IService<User> userService)
        {
            _logger = logger;
            _courseService = courseService;
            _userService = userService;
        }

        private static void GetSkills(User user, Course course)
        {
            foreach (var skill in course.Skills)
            {
                var userSkill = user.Profile.Skills.FirstOrDefault(s => s.Name == skill.Name);

                if (userSkill is not null)
                {
                    if ((int)userSkill.Level + (int)skill.Level >= 3)
                    {
                        userSkill.Level = Core.Enums.SkillLevelEnum.Advanced;
                    }
                    else
                    {
                        userSkill.Level = Core.Enums.SkillLevelEnum.Intermediate;
                    }
                }
                else
                {

                    user.Profile.Skills.Add(skill);
                }
            }
        }

        public async Task<IActionResult> CompleteMaterial(int courseId, int materialId, int materialTypeId)
        {
            var course = await _courseService.Get(courseId);

            

            if (course.Data.IsPersonal && HttpContext.User.FindFirst("CoursesIds").Value.Split(',').Contains(course.Data.Id.ToString()))
            {
                BaseMaterial material = course.Data.Videos.FirstOrDefault(m => m.Id == materialId && m.GetType() == _materialTypeDic[materialTypeId]) is not null? course.Data.Videos.FirstOrDefault(v => v.Id == materialId) :
                                           course.Data.Articles.FirstOrDefault(m => m.Id == materialId && m.GetType() == _materialTypeDic[materialTypeId]) is not null ? course.Data.Articles.FirstOrDefault(v => v.Id == materialId) :
                                           course.Data.Books.FirstOrDefault(m => m.Id == materialId && m.GetType() == _materialTypeDic[materialTypeId]) is not null ? course.Data.Books.FirstOrDefault(m => m.Id == materialId) : null;

                material.IsCompleted = true;

                course.Data.Progress = course.Data.GetProgres();

                if (course.Data.Progress == 100)
                {
                    var user = await _userService.Get(int.Parse(HttpContext.User.FindFirst("Id").Value));
                    GetSkills(user.Data, course.Data);
                    await _userService.Update(user.Data);
                }

                await _courseService.Update(course.Data);

                return RedirectToAction("View", "Course", new { id = course.Data.Id });
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> ViewAll(CoursesModel model)
        {
            var searchString = model.Search;
            var category = model.Category;
            var response = await _courseService.GetAll();
            var courses = response.Data;

            if (category > 0)
            {
                courses = courses.Where(c => c.CategoryId == category).ToList();
            }

            if (searchString is not null)
            {
                courses = courses.Where(c => c.Name.ToLower().Contains(searchString.ToLower()) |
                c.Description.ToLower().Contains(searchString.ToLower()) |
                c.Skills.Any(s => s.Name.ToLower().Contains(searchString.ToLower()) | s.Description.ToLower().Contains(searchString.ToLower())))
                .ToList();
            }

            List<int> ids = new();

            foreach(var course in courses)
            {
                ids.Add(course.Id);
            }

            var stringIds = string.Join(";", ids);

            if(stringIds.Length == 0)
            {
                stringIds = "empty";
            }

            return RedirectToAction("ViewAll", "Course", new { ids = stringIds });
        }

        public async Task<IActionResult> ViewAll(string ids = null)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _courseService.GetAll();
            var courses = response.Data;

            if (ids is not null) 
            {
                if (ids != "empty")
                {
                    var idsList = ids.Split(";");
                    var sortedCourses = courses.Where(c => idsList.Contains(c.Id.ToString())).ToList();
                    return View(new CoursesModel() { Data = sortedCourses });
                }
                else
                {
                    return View(new CoursesModel() { Data = new List<Course>() });
                }
            }

            _logger.LogInformation("Get All Course -> Ok");


            return View(new CoursesModel() {Data = courses.OrderBy(c => c.Name).ToList() });        
        }

        public async Task<IActionResult> View(int id) 
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _courseService.Get(id);
            if (response.Data.IsPersonal)
            {
                if (HttpContext.User.FindFirst("CoursesIds").Value.Split(',').Contains(response.Data.Id.ToString()))
                {
                    _logger.LogInformation("View Course -> Ok");
                    return View(response.Data);
                }
                else
                {
                    var responceCourses = await _courseService.GetAll();
                    var course = responceCourses.Data.FirstOrDefault(c => c.Name == response.Data.Name);

                    if (course is not null)
                    {
                        _logger.LogInformation("View Course -> Ok");
                        return View(course);
                    }
                }
                return RedirectToAction("Index", "Home");
            }

            _logger.LogInformation("View Course -> Ok");
            return View(response.Data);
        }

        public async Task<IActionResult> AddCourseToProfileAsync(int id)
        {
            var responce = await _courseService.Get(id);
            var newCourse = new Course();
            ICollection<VideoMaterial> newVideos = new List<VideoMaterial>();
            ICollection<BookMaterial> newBooks = new List<BookMaterial>();
            ICollection<ArticleMaterial> newArticles = new List<ArticleMaterial>();

            foreach (var video in responce.Data.Videos)
            {
                var newVideo = new VideoMaterial() { Name = video.Name, Description = video.Description, 
                    Resolution = video.Resolution, Source = video.Source, CreatedDate = video.CreatedDate };
                newVideos.Add(newVideo);
            }

            foreach (var book in responce.Data.Books)
            {
                var newBook = new BookMaterial() { Name = book.Name, Description = book.Description, Author = book.Author, 
                    PagesAmount = book.PagesAmount, Source = book.Source, CreatedDate = book.CreatedDate, Format = book.Format, PublishingDate = book.PublishingDate };
                newBooks.Add(newBook);
            }

            foreach (var article in responce.Data.Articles)
            {
                var newArticle = new ArticleMaterial() { Name = article.Name, Description = article.Description, 
                    Source = article.Source, CreatedDate = article.CreatedDate };
                newArticles.Add(newArticle);
            }

            newCourse.IsPersonal = true;
            newCourse.Name = responce.Data.Name;
            newCourse.Description = responce.Data.Description;
            newCourse.AuthorId = responce.Data.AuthorId;
            newCourse.CreatedDate = responce.Data.CreatedDate;
            newCourse.Skills = responce.Data.Skills;
            newCourse.Videos = newVideos;
            newCourse.Articles = newArticles;
            newCourse.Books = newBooks;
            newCourse.AvatarName = responce.Data.AvatarName;
            newCourse.Category = responce.Data.Category;
            newCourse.CategoryId = responce.Data.CategoryId;
            newCourse.IsCompleted = responce.Data.IsCompleted;

            var responceUser = await _userService.Get(int.Parse(HttpContext.User.FindFirst("Id").Value));
            responceUser.Data.Profile.Courses.Add(newCourse);

            await _userService.Update(responceUser.Data);

            var updatedUser = await _userService.Get(int.Parse(HttpContext.User.FindFirst("Id").Value));

            var claims = AuthHelper.Authenticate(updatedUser.Data);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claims));

            return RedirectToAction("Profile", "Account");
        }

        [HttpPost]
        public IActionResult AddVideo([Bind("Videos")] Course course)
        {
            course.Videos.Add(new VideoMaterial() { });
            return PartialView("Videos", course);
        }

        [HttpPost]
        public IActionResult AddBook([Bind("Books")] Course course)
        {
            course.Books.Add(new BookMaterial() {Source = "s" });
            return PartialView("Books", course);
        }

        [HttpPost]
        public IActionResult AddSkill([Bind("Skills")] Course course)
        {
            course.Skills.Add(new Skill() { });
            return PartialView("Skills", course);
        }

        [HttpPost]
        public IActionResult AddArticle([Bind("Articles")] Course course)
        {
            course.Articles.Add(new ArticleMaterial() { });
            return PartialView("Articles", course);
        }

        public async Task<IActionResult> AddOrEdit(int id = 0, Course course = null)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == 0)
                return View(new Course());

            if (id == -1)
                return View(course);

            else
            {
                var response = await _courseService.Get(id);

                if (response.Data.IsPersonal)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (HttpContext.User.FindFirst("Id").Value == response.Data.AuthorId.ToString())
                {
                    return View(response.Data);
                }

                return RedirectToAction("Index", "Home");
            }    
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Name,Description,AuthorId,File,AvatarName,Category,CategoryId,CreatedDate,Videos,Books,Articles,Skills")]Course course)
        {
            if (ModelState.IsValid)
            {
                if (course.Id == 0)
                {
                    course.AuthorId = int.Parse(HttpContext.User.FindFirst("Id").Value.ToString());
                    var reposnce  = await _courseService.Add(course);
                    if (reposnce.StatusCode == Core.Enums.StatusCode.Error)
                    {
                        _logger.LogError("AddOrEdit Course -> Error");
                        return RedirectToAction("AddOrEdit");
                    }
                }
                else
                {
                    var resposnce = await _courseService.Update(course);

                    if (resposnce.StatusCode == Core.Enums.StatusCode.Error)
                    {
                        _logger.LogError("AddOrEdit Course -> Error");
                        return RedirectToAction("AddOrEdit", new { id = course.Id });
                    }
                }
                   

                _logger.LogInformation("AddOrEdit Course -> Ok");
                return RedirectToAction("View", new { id = course.Id });
            }

            // error
            _logger.LogError("AddOrEdit Course -> Error");
            return RedirectToAction(nameof(ViewAll));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                await _courseService.Delete(id);

                _logger.LogInformation("Delete Course -> Ok");
                return RedirectToAction("ViewAll");
            }

            // error
            _logger.LogError("AddOrEdit Course -> Error");
            return RedirectToAction(nameof(ViewAll));
        }
    }
}
