using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineshop.Models;
using onlineshop.Repositroy;

namespace onlineshop.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ICategory _category;

        public CategoryController(ICategory category)
        {
            _category = category;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _category.GetAllCategory();
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create_CategoryVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _category.CreateCategory(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(result.Message);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid? id)
        {
            var category = await _category.GetCategoryById(id);
            if (category != null)
            {
                return View(category);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Update_CategoryVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _category.UpdateCategory(model);
                if (result.IsSuccess)
                {
                   return  RedirectToAction(result.Message);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {

                var result = await _category.DeleteCategory(id);
            if (result.IsSuccess)
            {
                return RedirectToAction(result.Message);
            }
            
            return BadRequest(result.Message);
        }
    } 
}
