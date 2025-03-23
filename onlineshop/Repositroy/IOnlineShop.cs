using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface IOnlineShop
    {
        Task<ResponseVM> GetAllProducts(int page, int pageSize);
        Task<ResponseVM> GetProductById(Guid id);
        Task<ResponseVM> CreateNewOrder(string id);
        Task<ResponseVM> AddNewTempBasket(T_TempOrder tempOrder, T_Product product, int quantity);
        ResponseVM UpdateTempBasket(T_TempBasket tempBasket, T_Product product, int quantity);
        void DeleteTempBasket(T_TempBasket basket);
        void DeleteTempOrder(T_TempOrder tempOrder);
        Task<Guid> AddOrder(CheckoutViewModel model, string userId);
        Task AddBasket(List<T_TempBasket> tempBasket, Guid orderId);
        Task<ResponseVM> GetAllByOrder(Guid orderId);
        Task<ResponseVM> ProductDetail(Guid productId);
        Task<ResponseVM> GetOrderByUserId(string userId);
        Task<ResponseVM> GetAllOrderByUserId(string userId);
        Task<ResponseVM> GetOrder(string orderId);
        Task<ResponseVM> SearchProduct(string filter);
        void AddPayment(T_Payment payment);
        void UpdateOrder(T_Order order);
        void Savechanges();
        Task saveChangesAsync();
    }
}
