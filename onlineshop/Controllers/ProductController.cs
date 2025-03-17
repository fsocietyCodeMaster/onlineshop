using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using onlineshop.Models;
using onlineshop.Repositroy;

namespace onlineshop.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        private readonly IProduct _product;
        private readonly ICategory _category;

        public ProductController(IProduct product,ICategory category)
        {
            _product = product;
            _category = category;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _product.GetAllProducts();
            return View(products.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _category.GetAllCategory();
            var result = categories.Data as List<T_L_Category>;
            ViewBag.Categories = new SelectList(categories.Data as List<T_L_Category>, "ID_Category", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create_ProductVM model)
        {

            var categories = await _category.GetAllCategory();
            ViewBag.Categories = new SelectList(categories.Data as List<T_L_Category>, "ID_Category", "Name");
            if (ModelState.IsValid)
                {
                var result = await _product.CreateProduct(model);
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
        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _product.GetProductById(id);
           var result = product.Data as T_Product;
            if (product != null)
            {
                var newProduct = new Update_ProductVM
                {
                    ID_Product = result.ID_Product,
                    Name = result.Name,
                    Description = result.Description,
                    Quantity = result.Quantity,
                    Price = result.Price,
                    IsAvailable = result.IsAvailable,
                    IsDiscountActive = result.IsDiscountActive,
                    Discount = result.Discount,
                    
                };
                var categories = await _category.GetAllCategory();
                ViewBag.Categories = new SelectList(categories.Data as List<T_L_Category>, "ID_Category", "Name");
                return View(newProduct);
            }
            return NotFound(product.Message);
        }


        [HttpPost]
        public async  Task<IActionResult> Update(Guid id , Update_ProductVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _product.UpdateProduct(id, model);
                if(result.IsSuccess)
                {
                    var categories = await _category.GetAllCategory();
                    ViewBag.Categories = new SelectList(categories.Data as List<T_L_Category>, "ID_Category", "Name");
                    return RedirectToAction(result.Message);
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
            var product =  await _product.DeleteProduct(id);
            if (product.IsSuccess)
            {
                return RedirectToAction(product.Message);
            }
            return NotFound(product.Message);
        }
    }
}
