using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using onlineshop.Context;
using onlineshop.Models;
using onlineshop.Repositroy;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace onlineshop.Services
{
    public class UserService : IUser
    {
        private readonly UserManager<T_User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<T_User> _signInManager;
        private readonly IConfiguration _configuration;
        public UserService(UserManager<T_User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<T_User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<ResponseVM> DeleteAsync(string id)
        {
            var userExist = await _userManager.FindByIdAsync(id);
            if (userExist != null)
            {
                var result = await _userManager.DeleteAsync(userExist);
                if (result.Succeeded)
                {
                    var success = new ResponseVM
                    {
                        IsSuccess = true
                    };
                    return success;
                }
                else
                {
                    var error = new ResponseVM
                    {
                        Message = "There is a problem while deleting the user.",
                        IsSuccess = false
                    };
                    return error;
                }
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<ResponseVM> LoginAsync(LoginVM login)
        {
            var userLogin = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, true);
            if (userLogin.Succeeded)
            {
                var success = new ResponseVM
                {
                    Message = login.ReturnUrl,
                    IsSuccess = true
                };
                return success;
            }
            if (userLogin.IsLockedOut)
            {
                var response = new ResponseVM
                {
                    Message = "Your account is locked out due to too many failed login attempts. Please try again later.",
                    IsSuccess = false

                };
                return response;
            }

            var error = new ResponseVM
            {
                Message = "Invalid Credentials.",
                IsSuccess = false
            };
            return error;

        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ResponseVM> RegisterAsync(RegisterVM model)
        {
            if (model != null)
            {
                var identityUser = new T_User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Age = model.Age,
                };
                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (result.Succeeded)
                {
                    var roleExists = await _roleManager.RoleExistsAsync("User");
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }
                }
                else
                {
                    var error = new ResponseVM
                    {
                        Data = result.Errors.Select(c => c.Description).ToList(),
                        IsSuccess = false

                    };
                }
                await _userManager.AddToRoleAsync(identityUser, "User");
                var success = new ResponseVM
                {
                    IsSuccess = true
                };
                return success;

            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "There is a problem sending data.",
                    IsSuccess = false
                };
                return error;

            }
        }

        public async Task<ResponseVM> UpdateAsync(Update_UserVM model)
        {
            var userExist = await _userManager.FindByIdAsync(model.CurrentUser.Id);
            if (userExist != null)
            {
                userExist.FullName = model.CurrentUser.FullName;
                userExist.Address = model.CurrentUser.Address;
                userExist.PhoneNumber = model.CurrentUser.PhoneNumber;
                userExist.Age = model.CurrentUser.Age;
                var result = await _userManager.UpdateAsync(userExist);
                if (result.Succeeded)
                {
                    var success = new ResponseVM
                    {
                        IsSuccess = true
                    };
                    return success;
                }
                else
                {
                    var error = new ResponseVM
                    {
                        Data = result.Errors.Select(c => c.Description).ToList(),
                        IsSuccess = false
                    };
                    return error;
                }
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public List<T_User> UsersList()
        {
            var users = _userManager.Users.ToList();
            return users;
        }
    }
}
