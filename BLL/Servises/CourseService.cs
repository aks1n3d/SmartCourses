using BLL.Servises.Abstractions;
using Core.Entity;
using Core.Enums;
using Core.Response;
using Core.Response.Abstractions;
using DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;

namespace BLL.Servises
{
    public class CourseService : IService<Course>
    {
        private IBaseRepository<Course> _repository { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CourseService(IBaseRepository<Course> repository, IWebHostEnvironment hostingEnvironment)
        {
            _repository = repository;
            _hostingEnvironment = hostingEnvironment;
        }

        private static string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        private void ProccesTheContent(Course entity)
        {
            if (entity.File is not null)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                entity.AvatarName = GetUniqueFileName(entity.File.FileName);
                var filePath = Path.Combine(uploads, entity.AvatarName);
                entity.File.CopyTo(new FileStream(filePath, FileMode.Create)); 
            }

            if (entity.Books.Count != 0)
            {
                foreach (var book in entity.Books)
                {
                    if (book.BookFile is not null)
                    {
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                        //if (book.Source != null)
                        //{
                        //    var fileToDeletePath = Path.Combine(uploads, book.Source);
                        //    FileInfo fileToDelete = new FileInfo(fileToDeletePath);

                        //    if (fileToDelete.Exists)
                        //    {
                        //        fileToDelete.Delete();
                        //    }
                        //}

                        book.Source = GetUniqueFileName(book.BookFile.FileName);
                        book.Format = Path.GetExtension(book.Source);
                        var filePath = Path.Combine(uploads, book.Source);
                        book.BookFile.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
            }
            
            if (entity.Videos.Count != 0)
            {
                foreach(var video in entity.Videos)
                {
                    if (video.Source.Contains("v="))
                    {
                        video.Source = video.Source.Split("v=")[1];
                    } 
                }
            }
        }

        public async Task<IBaseResponse<bool>> Add(Course entity)
        {
            IBaseResponse<bool> response = new BaseResponse<bool>();

            ProccesTheContent(entity);
            entity.GetProgres();

            if (await _repository.Create(entity))
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Курс добавлен";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Курс не добавлен";
                response.Data = false;
            }

            return response;
        }

        public async Task<IBaseResponse<bool>> Delete(int id)
        {
            IBaseResponse<bool> response = new BaseResponse<bool>();

            if (await _repository.Delete(id))
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Курс удален";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Курс не удален";
                response.Data = false;
            }

            return response;
        }

        public async Task<IBaseResponse<Course>> Get(int id)
        {
            IBaseResponse<Course> response = new BaseResponse<Course>();

            Course course = await _repository.Get(id);

            if (course is not null)
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Курс найден";
                response.Data = course;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Курс не найден";
                response.Data = null;
            }

            return response;
        }

        public async Task<IBaseResponse<ICollection<Course>>> GetAll()
        {
            IBaseResponse<ICollection<Course>> response = new BaseResponse<ICollection<Course>>();

            ICollection<Course> courses = await _repository.GetAll();

            if (courses.Count != 0)
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Курсы найдены";
                response.Data = courses;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Курсы не найдены";
                response.Data = courses;
            }

            return response;
        }

        public async Task<IBaseResponse<bool>> Update(Course entity)
        {
            IBaseResponse<bool> response = new BaseResponse<bool>();

            ProccesTheContent(entity);
            entity.GetProgres();

            if (await _repository.Update(entity))
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Курс изменен";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Курс не изменен";
                response.Data = false;
            }

            return response;
        }
    }
}
