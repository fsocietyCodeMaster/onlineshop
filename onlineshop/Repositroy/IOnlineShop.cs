using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface IOnlineShop
    {
        Task<ResponseVM> GetAllProducts(int page, int pageSize);
        Task<ResponseVM> GetProductById(Guid id);
        Task<ResponseVM> GetTempOrderByUser(string id);
        Task<ResponseVM> CreateNewOrder(string id);
        Task<ResponseVM> GetTempOrderById(Guid tempOrderId, Guid productId);
        Task<ResponseVM> AddNewTempBasket(T_TempOrder tempOrder,T_Product product, int quantity);
        void UpdateTempBasket(T_TempBasket tempBasket,T_Product product, int quantity);
        Task<ResponseVM> GetTempBasketByOrderId(Guid id);
        void DeleteTempBasket(T_TempBasket basket);
        void DeleteTempOrder(T_TempOrder tempOrder);
        Task<Guid> AddOrder(CheckoutViewModel model,string userId);
        Task AddBasket(List<T_TempBasket> tempBasket,Guid orderId);
        Task<ResponseVM> GetAllByOrder(Guid  orderId);
        Task<ResponseVM> GetTempBasket(Guid  tempbasket);
        Task<ResponseVM> ProductDetail(Guid productId);
        Task<ResponseVM> GetOrderByUserId(string userId);
        Task<ResponseVM> GetOrder(string orderId);
        void AddPayment(T_Payment payment);
        void UpdateOrder(T_Order order);
        void Savechanges();
        Task saveChangesAsync();
    }
}
