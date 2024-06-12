using BLL.Servises.Abstractions;
using Core.Entity;
using Core.Enums;
using Core.Response;
using Core.Static;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Servises
{
    public class AccountService : IAccountService
    {
        private IService<User> _userService { get; set; }

        public AccountService(IService<User> userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            var users = await _userService.GetAll();
            var user = users.Data.FirstOrDefault(u => u.Login == model.Login);

            if (user is not null)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Info = "There is user with the same login",
                    StatusCode = StatusCode.Error
                };
            }

            Profile profile = new Profile();

            user = new User()
            {
                Login = model.Login,
                Password = HashPasswordHelper.HashPassword(model.Password),
                Profile = profile
            };

            await _userService.Add(user);

            var result = AuthHelper.Authenticate(user);

            return new BaseResponse<ClaimsIdentity>()
            {
                Data = result,
                Info = "Registered",
                StatusCode = StatusCode.Ok
            };
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            var users = await _userService.GetAll();
            var user = users.Data.FirstOrDefault(u => u.Login == model.Login);

            if (user is null)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Info = "There is no user with this login",
                    StatusCode = StatusCode.Error
                };
            }

            if (user.Password != HashPasswordHelper.HashPassword(model.Password))
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Info = "Password is not correct",
                    StatusCode = StatusCode.Error
                };
            }

            var result = AuthHelper.Authenticate(user);

            return new BaseResponse<ClaimsIdentity>()
            {
                Data = result,
                Info = "Registered",
                StatusCode = StatusCode.Ok
            };


        }
    }
}
