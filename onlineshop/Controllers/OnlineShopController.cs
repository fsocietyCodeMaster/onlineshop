using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using onlineshop.Helper;
using onlineshop.Models;
using onlineshop.Repositroy;
using System.Security.Claims;


namespace onlineshop.Controllers
{
    public class OnlineShopController : Controller
    {
        private readonly IOnlineShop _onlineShop;
        private readonly IProduct _product;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;

        public OnlineShopController(IOnlineShop onlineShop, IProduct product, IMapper mapper, IHttpClientFactory clientFactory)
        {
            _onlineShop = onlineShop;
            _product = product;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }
        [HttpPost]
        public async Task<IActionResult> AddToBasket(Guid id, int quantity)
        {
            #region user-product
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _product.GetProductById(id);
            #endregion
            #region condition
            var productConvert = product.Data as T_Product;
            if (product == null || !productConvert.IsAvailable || productConvert.Quantity == 0)
            {
                return View("ProductNotFound");
            }
            else if (quantity > productConvert.Quantity)
            {
                ViewBag.name = productConvert.Name;
                return View("LimitedProduct");
            }
            #endregion
            #region TempOrder
            var tempOrder = HttpContext.Session.GetObject<T_TempOrder>("tempOrder");
            if (tempOrder == null)
            {
                var result = await _onlineShop.CreateNewOrder(user);
                tempOrder = result.Data as T_TempOrder;
                HttpContext.Session.SetObject("tempOrder", tempOrder);
            }
            #region TempBasket
            var basketList = HttpContext.Session.GetObject<List<T_TempBasket>>("tempBasket") ?? new List<T_TempBasket>();
            var existingItem = basketList.FirstOrDefault(c => c.T_Product_ID == productConvert.ID_Product);
            if (existingItem == null)
            {
                var result = await _onlineShop.AddNewTempBasket(tempOrder, productConvert, quantity);
                var newItem = result.Data as T_TempBasket;
                basketList.Add(newItem);
            }
            else
            {
              
              var tempList =  _onlineShop.UpdateTempBasket(existingItem,existingItem.Product, quantity);
                HttpContext.Session.SetObject("tempBasket", tempList.Data);

            }
            HttpContext.Session.SetObject("tempBasket", basketList);

            #endregion
            #endregion

            return RedirectToAction("ShowBasket");
        }

        [HttpGet]
        public  IActionResult ShowBasket()
        {
            #region show basket
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tempOrderSession = HttpContext.Session.GetObject<T_TempOrder>("tempOrder");
            if (tempOrderSession == null)
            {
                return View();
            }

            var expirationTime = TimeSpan.FromMinutes(3);
            var currentTime = DateTime.Now;
            var elapsed = currentTime - tempOrderSession.CreatedAt;
            var tempBasketSession = HttpContext.Session.GetObject<List<T_TempBasket>>("tempBasket");
            if (elapsed >= expirationTime)
            {
                HttpContext.Session.Remove("tempBasket");
                HttpContext.Session.Remove("tempOrder");
                return RedirectToAction("ShowBasket");

            }
            return View(tempBasketSession);
            #endregion

        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            #region user-order-basket
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tempOrder = HttpContext.Session.GetObject<T_TempOrder>("tempOrder");
            var tempBasket = HttpContext.Session.GetObject<List<T_TempBasket>>("tempBasket");
            var tempBasketFiltered = tempBasket.Where(c => c.T_tempOrder_ID == tempOrder.ID_TempOrder).ToList();
            #endregion
            #region checking items
            if (tempBasketFiltered == null)
            {
                return NotFound();
            }
            #endregion
            #region creating order
            var order = await _onlineShop.AddOrder(model, user);
            #endregion
            #region creating basket
            await _onlineShop.AddBasket(tempBasketFiltered, order);
            #endregion
            #region delete basket
            HttpContext.Session.Remove("tempBasket");
            HttpContext.Session.Remove("tempOrder");
            #endregion
            return RedirectToAction("Showorder"); // can goes to showorder
        }

        [HttpGet]
        public async Task<IActionResult> Showorder()
        {
            #region user-order

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _onlineShop.GetOrderByUserId(user);
            var orderConverted = order.Data as T_Order;
            if (orderConverted == null)
            {
                return NotFound();
            }
            #endregion
            #region mapping basket
            var details = await _onlineShop.GetAllByOrder(orderConverted.ID_Order);
            var detailsConverted = details.Data as List<T_Basket>;
            var orderDetails = _mapper.Map<List<BasketViewModel>>(detailsConverted);
            #endregion
            #region calculating the totalprice
            long totalPrice = 0;
            foreach (var item in orderDetails)
            {
                if (item.Product.IsDiscountActive && item.Product.Discount.HasValue)
                {
                    totalPrice += DiscountExtention.GetDiscount(item.Product) * item.Quantity;

                }
                else
                {
                    totalPrice += item.Product.Price * item.Quantity;
                }
            }

            #endregion
            #region mapping order
            var finalOrder = new OrderViewModel
            {
                Address = orderConverted.Address,
                City = orderConverted.City,
                Name = orderConverted.Name,
                BasketDetails = orderDetails,
                OrderStatus = orderConverted.OrderStatus,

            };
            ViewBag.price = totalPrice;
            return View(finalOrder);
            #endregion
        }
        [HttpGet]
        public async Task<IActionResult> ShowAllOrders()
        {
            #region user-order

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _onlineShop.GetAllOrderByUserId(user);
            var orderConverted = order.Data as List<T_Order>;
            if (orderConverted == null)
            {
                return NotFound();
            }
            #endregion
            return View(orderConverted);
        }




        [HttpPost]
        public  IActionResult DeleteItems(Guid tempBasket)
        {
            #region delete items 

            var tempBasketSession = HttpContext.Session.GetObject<List<T_TempBasket>>("tempBasket").FirstOrDefault(c=> c.ID_TempBasket == tempBasket);
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (tempBasketSession == null)
            {
                return RedirectToAction("ShowBasket");
            }
            HttpContext.Session.Remove("tempBasket");
            return RedirectToAction("ShowBasket");
            #endregion
        }

        [HttpPost]
        public  IActionResult EditItems(Guid tempBasket, int quantity)
        {
            #region edit item
            var tempBasketSession = HttpContext.Session.GetObject<List<T_TempBasket>>("tempBasket");
            var existingItem = tempBasketSession.FirstOrDefault(c => c.ID_TempBasket == tempBasket);
            if (existingItem == null)
            {
                return NotFound();
            }
            var tempList = _onlineShop.UpdateTempBasket(existingItem, existingItem.Product, quantity);
            HttpContext.Session.SetObject("tempBasket", tempList.Data);
            return RedirectToAction("ShowBasket");
            #endregion
        }

        [HttpGet]
        public IActionResult ProductDetail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDetail(Guid productId)
        {
            var product = await _onlineShop.ProductDetail(productId);
            if (product.IsSuccess)
            {
                return View(product.Data);
            }
            else
            {
                return NotFound(product.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment()
        {

            #region Creating trackid
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _onlineShop.GetOrderByUserId(user);
            var orderConverted = order.Data as T_Order;
            if (order == null)
            {
                return NotFound();
            }
            var data = new
            {
                merchant = "zibal",
                amount = orderConverted.Baskets.Sum(c => c.TotalPrice),
                callbackUrl = "https://localhost:7005/OnlineShop/OnlinePayment/",
                orderId = orderConverted.ID_Order,
                description = "Payment factor"

            };
            var jasonString = JsonConvert.SerializeObject(data);
            var client = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.zibal.ir/v1/request");
            var content = new StringContent(jasonString, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(payload);
            jsonObject.TryGetValue("trackId", out var trackIdValue);
            var trackId = trackIdValue;
            jsonObject.TryGetValue("result", out var resultIdValue);
            var resultId = resultIdValue;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Payment request failed with status code: {response.StatusCode}");
                return BadRequest("Payment request failed.");
            }
            if (resultId.ToString() == "100" && trackId != null)
            {
                var paymentPage = $"https://gateway.zibal.ir/start/{trackId}";
                return Redirect(paymentPage);
            }
            else
            {
                Console.WriteLine($"Payment request failed with status code: {response.StatusCode}");
                return BadRequest("Payment request failed.");
            }
            #endregion
        }

        [HttpGet]
        public async Task<IActionResult> OnlinePayment()
        {
            #region online payment
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var status = HttpContext.Request.Query["Status"];
            var trackId = HttpContext.Request.Query["trackId"];
            var orderId = HttpContext.Request.Query["orderId"];
            long.TryParse(trackId, out var longTrackid);
            var newTrackid = longTrackid;
            if (!string.IsNullOrEmpty(status) && (!string.IsNullOrEmpty(trackId) && (!string.IsNullOrEmpty(orderId))))
            {

                var order = await _onlineShop.GetOrder(orderId);
                var orderConverted = order.Data as T_Order;
                if (orderConverted == null)
                {
                    return BadRequest();
                }

                var Data = new
                {
                    merchant = "zibal",
                    trackId = newTrackid,
                };
                var jsonString = JsonConvert.SerializeObject(Data);
                var client = _clientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.zibal.ir/v1/verify");
                var content = new StringContent(jsonString, null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var paymentResponses = JsonConvert.DeserializeObject<PaymentViewModel>(responseString);
                var jsonObject = JObject.Parse(responseString);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Verification failed with status code: {response.StatusCode}");
                    return BadRequest("Payment verification failed.");
                }
                jsonObject.TryGetValue("result", out var resultValue);
                var resultResponse = resultValue.ToString();
                var paymentResult = new T_Payment()
                {
                    Message = paymentResponses.message,
                    Description = paymentResponses.description,
                    T_Order_ID = paymentResponses.orderId,
                    PaidAt = paymentResponses.PaidAt,
                    TrackId = newTrackid
                };
                if (resultResponse == "100")
                {
                    orderConverted.IsFinal = true;
                    orderConverted.OrderStatus = OrderStatus.PROCESSING;
                    foreach (var products in orderConverted.Baskets)
                    {
                        products.Product.Quantity -= products.Quantity;
                    }
                    _onlineShop.UpdateOrder(orderConverted);
                    await _onlineShop.saveChangesAsync();
                    _onlineShop.AddPayment(paymentResult);
                    await _onlineShop.saveChangesAsync();

                    ViewBag.code = trackId;
                    return View();
                }
                else
                {
                    Console.WriteLine($"Verification failed with status code: {response.StatusCode}");
                    return BadRequest("Payment verification failed.");
                }

            }
            return NotFound();
            #endregion
        }
    }
}
