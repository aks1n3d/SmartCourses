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
using Microsoft.Extensions.Hosting;

namespace BLL.Servises
{
    public class UserService : IService<User>
    {
        private IBaseRepository<User> _repository { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserService(IBaseRepository<User> repository, IWebHostEnvironment hostingEnvironment)
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

        private void ProccesTheContent(Profile entity)
        {
            if (entity.File is not null)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                entity.AvatarName = GetUniqueFileName(entity.File.FileName);
                var filePath = Path.Combine(uploads, entity.AvatarName);
                entity.File.CopyTo(new FileStream(filePath, FileMode.Create));
            }
        }

        public async Task<IBaseResponse<bool>> Add(User user)
        {
            IBaseResponse<bool> response = new BaseResponse<bool>();

            if (await _repository.Create(user))
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Пользователь добавлен";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Пользователь не добавлен";
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
                response.Info = "Пользователь удален";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Пользователь не удален";
                response.Data = false;
            }

            return response;
        }

        public async Task<IBaseResponse<User>> Get(int id)
        {
            IBaseResponse<User> response = new BaseResponse<User>();

            User user = await _repository.Get(id);

            if (user is not null)
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Пользователь найден";
                response.Data = user;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Пользователь не найден";
                response.Data = null;
            }

            return response;
        }

        public async Task<IBaseResponse<ICollection<User>>> GetAll()
        {
            IBaseResponse<ICollection<User>> response = new BaseResponse<ICollection<User>>();

            ICollection<User> users = await _repository.GetAll();

            if (users.Count != 0)
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Пользователи найдены";
                response.Data = users;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Пользователи не найдены";
                response.Data = users;
            }

            return response;
        }

            public async Task<IBaseResponse<bool>> Update(User user)
        {
            IBaseResponse<bool> response = new BaseResponse<bool>();

            ProccesTheContent(user.Profile);

            if (await _repository.Update(user))
            {
                response.StatusCode = StatusCode.Ok;
                response.Info = "Пользователь изменен";
                response.Data = true;
            }
            else
            {
                response.StatusCode = StatusCode.Error;
                response.Info = "Пользователь не изменен";
                response.Data = false;
            }

            return response;
        }
    }
}
