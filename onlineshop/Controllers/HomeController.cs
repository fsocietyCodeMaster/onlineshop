using Microsoft.AspNetCore.Mvc;
using onlineshop.Models;
using onlineshop.Repositroy;
using X.PagedList;


namespace onlineshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOnlineShop _onlineShop;

        public HomeController(ILogger<HomeController> logger, IOnlineShop onlineShop)
        {
            _logger = logger;
            _onlineShop = onlineShop;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var products = await _onlineShop.GetAllProducts(pageNumber, pageSize);
            return View((products.Data as IPagedList<T_Product>));
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
