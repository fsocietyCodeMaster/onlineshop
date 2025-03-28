using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using onlineshop.Models;
using onlineshop.Repositroy;
using System.Reflection;

namespace onlineshop.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _user;
        private readonly UserManager<T_User> _userManager;
        public UserController(IUser user, UserManager<T_User> userManager)
        {
            _user = user;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = _user.UsersList();
            return View(users);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _user.RegisterAsync(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Login");
                }
                if (result.Data is List<string> errors)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }

            }
            return View(model);
        }

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl)
        {
            var url = new LoginVM { ReturnUrl = returnUrl };
            return View(url);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _user.LoginAsync(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(result.Message ?? "index","home"); // bayad ok beshe badan.
                }
                ModelState.AddModelError("", result.Message);

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser != null)
            {
                var userInfo = new Update_UserVM
                {
                    CurrentUser = currentUser
                };
                return View(userInfo);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Update(Update_UserVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _user.UpdateAsync(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                if (result.Data is List<string> errors)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (ModelState.IsValid)
            {
                var result = await _user.DeleteAsync(id);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
           await _user.Logout();
            return RedirectToAction("index","home");
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}