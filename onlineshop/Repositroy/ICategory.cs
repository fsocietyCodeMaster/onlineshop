using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface ICategory
    {
        Task<ResponseVM> CreateCategory(Create_CategoryVM model);
        Task<ResponseVM> UpdateCategory(Update_CategoryVM model);
        Task<Update_CategoryVM> GetCategoryById(Guid? id);
        Task<ResponseVM> GetAllCategory();
        Task<ResponseVM> DeleteCategory(Guid id);
    }
}
