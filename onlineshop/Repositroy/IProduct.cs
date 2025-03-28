using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface IProduct
    {
        Task<ProductResultVM> CreateProduct(Create_ProductVM model);
        Task<ProductResultVM> GetAllProducts();
        Task<ProductResultVM> UpdateProduct(Guid id, Update_ProductVM model);
        Task<ProductResultVM> GetProductById(Guid id);
        Task<ProductResultVM> DeleteProduct(Guid id);

    }
}
