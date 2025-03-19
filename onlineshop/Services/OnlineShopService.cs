using Microsoft.EntityFrameworkCore;
using onlineshop.Context;
using onlineshop.Helper;
using onlineshop.Models;
using onlineshop.Repositroy;
using X.PagedList.Extensions;
namespace onlineshop.Services
{
    public class OnlineShopService : IOnlineShop
    {
        private readonly OnlineShopDb _context;

        public OnlineShopService(OnlineShopDb context)
        {
            _context = context;
        }

        public async Task<ResponseVM> AddNewTempBasket(T_TempOrder tempOrder, T_Product product, int quantity)
        {
            if (product.IsDiscountActive && product.Discount.HasValue)
            {
                var discountPrice = DiscountExtention.GetDiscount(product);
                var tempBasketItem1 = new T_TempBasket
                {
                    Product = product,
                    Quantity = quantity,
                    TotalPrice = discountPrice * quantity,
                    T_tempOrder_ID = tempOrder.ID_TempOrder
                };
                _context.Add(tempBasketItem1);
                await _context.SaveChangesAsync();
                return new ResponseVM
                {
                    IsSuccess = true,
                    Data = tempBasketItem1
                };

            }
            else
            {
                var tempBasketItem2 = new T_TempBasket
                {
                    Product = product,
                    Quantity = quantity,
                    TotalPrice = quantity * product.Price,
                    T_tempOrder_ID = tempOrder.ID_TempOrder
                };
                _context.Add(tempBasketItem2);
                await _context.SaveChangesAsync();
                return new ResponseVM
                {
                    IsSuccess = true,
                    Data = tempBasketItem2
                };

            }
        }

        public void UpdateTempBasket(T_TempBasket tempBasket, T_Product product, int quantity)
        {
            int quantityDifference = quantity - tempBasket.Quantity;
            if (quantityDifference != 0)
            {
                var pricePerUnit = product.IsDiscountActive && product.Discount.HasValue ? DiscountExtention.GetDiscount(product) : product.Price;
                tempBasket.Quantity = quantity;
                tempBasket.TotalPrice = tempBasket.Quantity * pricePerUnit;
                _context.Update(tempBasket);
                _context.SaveChanges();
            }
            else
            {
                return;
            }

        }

        public async Task<ResponseVM> CreateNewOrder(string id)
        {
            var order = new T_TempOrder
            {
                T_User_ID = id,
                CreatedAt = DateTime.Now,
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
            return new ResponseVM
            {
                IsSuccess = true,
                Data = order
            };
        }

        public async Task<ResponseVM> GetAllProducts(int page, int pageSize)
        {
            var products = await _context.T_Product.Include(c => c.Photos).Where(c => c.IsAvailable == true && c.Category.IsActive == true).ToListAsync();
            if (products != null && products.Any())
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = products.ToPagedList(page, pageSize)
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no products."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetTempOrderByUser(string id)
        {
            var order = await _context.T_TempOrder.FirstOrDefaultAsync(c => c.T_User_ID == id);
            if (order != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = order
                };
                return success;
            }
            else
            {

                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no order."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetProductById(Guid id)
        {
            var product = await _context.T_Product.FirstOrDefaultAsync(c => c.ID_Product == id);
            if (product != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = product
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no product."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetTempOrderById(Guid tempOrderId, Guid productId)
        {
            var tempOrder = await _context.T_TempBasket.Include(c => c.Product).ThenInclude(c => c.Photos).FirstOrDefaultAsync(c => c.T_tempOrder_ID == tempOrderId && c.T_Product_ID == productId);
            if (tempOrder != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = tempOrder
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no temporder."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetTempBasketByOrderId(Guid id)
        {
            var tempBasket = await _context.T_TempBasket.Include(c => c.Product).ThenInclude(c => c.Photos).Where(c => c.T_tempOrder_ID == id).ToListAsync();
            if (tempBasket != null && tempBasket.Any())
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = tempBasket
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no tempbasket."
                };
                return error;
            }
        }

        public void DeleteTempBasket(T_TempBasket basket)
        {
            _context.Remove(basket);
        }

        public void DeleteTempOrder(T_TempOrder tempOrder)
        {
            _context.Remove(tempOrder);
        }

        public void Savechanges()
        {
            _context.SaveChanges();
        }

        public async Task<Guid> AddOrder(CheckoutViewModel model, string userId)
        {
            var order = new T_Order
            {
                Name = model.Name,
                City = model.City,
                Address = model.Address,
                OrderStatus = OrderStatus.PENDING,
                T_User_ID = userId,
                OrderDate = DateTime.Now,
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
            return order.ID_Order;
        }

        public async Task AddBasket(List<T_TempBasket> tempBasket, Guid orderId)
        {
            foreach (var item in tempBasket)
            {
                var orderDetail = new T_Basket
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    T_Order_ID = orderId,
                    TotalPrice = item.TotalPrice,
                };
                _context.Add(orderDetail);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ResponseVM> GetOrderByUserId(string userId)
        {
            var order = await _context.T_Order.Include(c => c.Baskets).ThenInclude(c => c.Product).ThenInclude(c => c.Photos).Where(c => c.T_User_ID == userId && c.IsFinal == false).FirstOrDefaultAsync();
            if (order != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = order
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no order."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetAllByOrder(Guid orderId)
        {
            var baskets = await _context.T_Basket.Include(c => c.Product).Where(c => c.T_Order_ID == orderId).ToListAsync();
            if (baskets != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = baskets
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no basket."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetTempBasket(Guid tempbasket)
        {
            var tempBasket = await _context.T_TempBasket.Include(c => c.Product).FirstOrDefaultAsync(c => c.ID_TempBasket == tempbasket);
            if (tempBasket != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = tempBasket
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no basket."
                };
                return error;
            }
        }

        public async Task<ResponseVM> ProductDetail(Guid productId)
        {
            var product = await _context.T_Product.Include(c => c.Photos).FirstOrDefaultAsync(c => c.ID_Product == productId);
            if (product != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = product
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no basket."
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetOrder(string orderId)
        {
            var order = await _context.T_Order.Include(c => c.Baskets).ThenInclude(c => c.Product).FirstOrDefaultAsync(c => c.ID_Order.ToString() == orderId && c.IsFinal == false);
            if (order != null)
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = order
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no basket."
                };
                return error;
            }
        }

        public void UpdateOrder(T_Order order)
        {
            _context.Update(order);
        }

        public async Task saveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void AddPayment(T_Payment payment)
        {
            _context.Add(payment);
        }

        public async Task<ResponseVM> GetAllOrderByUserId(string userId)
        {
            var orders = await _context.T_Order.Include(c => c.Baskets).ThenInclude(c => c.Product).ThenInclude(c => c.Photos).Where(c => c.T_User_ID == userId).ToListAsync();
            if (orders.Any())
            {
                var success = new ResponseVM
                {
                    IsSuccess = true,
                    Data = orders
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    IsSuccess = false,
                    Message = "There is no order."
                };
                return error;
            }
        }
    }
}
