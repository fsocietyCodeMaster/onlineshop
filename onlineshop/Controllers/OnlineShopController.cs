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
            var tempOrder = await _onlineShop.GetTempOrderByUser(user);
            var tempOrderConverted = tempOrder.Data as T_TempOrder;
            if (tempOrderConverted == null)
            {
                var result = await _onlineShop.CreateNewOrder(user);
                tempOrderConverted = result.Data as T_TempOrder;
            }
            #region TempBasket
            var tempBasket = await _onlineShop.GetTempOrderById(tempOrderConverted.ID_TempOrder, productConvert.ID_Product);
            var tempBasketConverted = tempBasket.Data as T_TempBasket;
            if (tempBasketConverted == null || tempBasketConverted.T_Product_ID != productConvert.ID_Product)
            {
                await _onlineShop.AddNewTempBasket(tempOrderConverted, productConvert, quantity);
            }
            else
            {
                _onlineShop.UpdateTempBasket(tempBasketConverted, productConvert, quantity); // when time pass it goes here i need to check.
            }
            #endregion
            #endregion

            return RedirectToAction("ShowBasket");
        }

        [HttpGet]
        public async Task<IActionResult> ShowBasket()
        {
            #region show basket
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tempOrder = await _onlineShop.GetTempOrderByUser(user);
            var tempOrderConverted = tempOrder.Data as T_TempOrder;

            if (tempOrderConverted == null)
            {
                return View();
            }

            var expirationTime = TimeSpan.FromMinutes(3);
            var currentTime = DateTime.Now;
            var elapsed = currentTime - tempOrderConverted.CreatedAt;
            if (elapsed >= expirationTime)
            {
                var tempBasket = await _onlineShop.GetTempBasketByOrderId(tempOrderConverted.ID_TempOrder);
                var tempBasketConverted = tempBasket.Data as List<T_TempBasket>;
                if (tempBasketConverted != null && tempBasketConverted.Any())
                {
                    foreach (var item in tempBasketConverted)
                    {
                        _onlineShop.DeleteTempBasket(item);
                    }
                    _onlineShop.Savechanges();

                }
                _onlineShop.DeleteTempOrder(tempOrderConverted);
                _onlineShop.Savechanges();

                return NotFound();

            }
            var basket = await _onlineShop.GetTempBasketByOrderId(tempOrderConverted.ID_TempOrder);
            var basketConverted = basket.Data as List<T_TempBasket>;
            return View(basketConverted);
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
            var tempOrder = await _onlineShop.GetTempOrderByUser(user);
            var tempOrderConverted = tempOrder.Data as T_TempOrder;
            var tempBasket = await _onlineShop.GetTempBasketByOrderId(tempOrderConverted.ID_TempOrder);
            var tempBasketConverted = tempBasket.Data as List<T_TempBasket>;
            #endregion
            #region checking items
            if (tempBasketConverted == null)
            {
                return NotFound();
            }
            #endregion
            #region creating order
            var order = await _onlineShop.AddOrder(model, user);
            #endregion
            #region creating basket
            await _onlineShop.AddBasket(tempBasketConverted, order);
            #endregion
            #region delete basket
            foreach (var item in tempBasketConverted)
            {
                _onlineShop.DeleteTempBasket(item);
            }
            await _onlineShop.saveChangesAsync();
            _onlineShop.DeleteTempOrder(tempOrderConverted);
            await _onlineShop.saveChangesAsync();
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
        public async Task<IActionResult> DeleteItems(Guid tempBasket)
        {
            #region delete items 
            var basketItems = await _onlineShop.GetTempBasket(tempBasket);
            var basketItemsConverted = basketItems.Data as T_TempBasket;
            if (basketItemsConverted == null)
            {
                return NotFound();
            }
            _onlineShop.DeleteTempBasket(basketItemsConverted);
            _onlineShop.Savechanges();
            return RedirectToAction("ShowBasket");
            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> EditItems(Guid tempBasket, int quantity)
        {
            #region edit item
            var basketItems = await _onlineShop.GetTempBasket(tempBasket);
            var basketItemsConverted = basketItems.Data as T_TempBasket;
            if (basketItemsConverted == null)
            {
                return NotFound();
            }
            var product = basketItemsConverted.Product;
            _onlineShop.UpdateTempBasket(basketItemsConverted, product, quantity);
            _onlineShop.Savechanges();
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
