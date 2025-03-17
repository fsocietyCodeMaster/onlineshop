using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface IProduct
    {
        Task<ResponseVM> CreateProduct(Create_ProductVM model);
        Task<ResponseVM> GetAllProducts();
        Task<ResponseVM> UpdateProduct(Guid id, Update_ProductVM model);
        Task<ResponseVM> GetProductById(Guid id);
        Task<ResponseVM> DeleteProduct(Guid id);

    }
}
