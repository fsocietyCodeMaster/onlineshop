using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface ICategory
    {
        Task<CategoryResultVM> CreateCategory(Create_CategoryVM model);
        Task<CategoryResultVM> UpdateCategory(Update_CategoryVM model);
        Task<Update_CategoryVM> GetCategoryById(Guid? id);
        Task<CategoryResultVM> GetAllCategory();
        Task<CategoryResultVM> DeleteCategory(Guid id);
    }
}
