using onlineshop.Context;
using onlineshop.Models;
using onlineshop.Repositroy;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace onlineshop.Services
{
    public class CategoryService : ICategory
    {
        private readonly OnlineShopDb _context;
        private readonly IMapper _mapper;

        public CategoryService(OnlineShopDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CategoryResultVM> CreateCategory(Create_CategoryVM model)
        {
            if (model != null)
            {
                bool exists = await _context.T_L_Category.AnyAsync(c => c.Name == model.Name);
                if (exists)
                    return new CategoryResultVM
                    {
                        Message = "Category already exists.",
                        IsSuccess = false
                    };

                var category = _mapper.Map<T_L_Category>(model);
                _context.Add(category);
                await _context.SaveChangesAsync();
                var success = new CategoryResultVM
                {
                    Message = "index",
                    IsSuccess = true
                };
                return success;
            }
            else
            {
                var error = new CategoryResultVM
                {
                    Message = "There is problem sending data.", // i didnt check errors.
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<CategoryResultVM> DeleteCategory(Guid id)
        {
            var category = await _context.T_L_Category.FindAsync(id);
            if (category != null)
            {
                _context.Remove(category);
                await _context.SaveChangesAsync();
                var success = new CategoryResultVM
                {
                    Message = "index",
                    IsSuccess = true
                };
                return success;
            }
            else
            {
                var error = new CategoryResultVM
                {
                    Message = "There is no category.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<CategoryResultVM> GetAllCategory()
        {
            var categories = await _context.T_L_Category.ToListAsync();
            if (categories.Any())
            {
                var success = new CategoryResultVM
                {
                    IsSuccess = true,
                    Categories = categories
                };
                return success;
            }
            else
            {
                var error = new CategoryResultVM
                {
                    Message = "There is no category.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<Update_CategoryVM> GetCategoryById(Guid? id)
        {
            var category = await _context.T_L_Category.FirstOrDefaultAsync(c => c.ID_Category == id);
            var result = _mapper.Map<Update_CategoryVM>(category);
            return result;

        }

        public async Task<CategoryResultVM> UpdateCategory(Update_CategoryVM model)
        {
            var category = await _context.T_L_Category.FindAsync(model.ID_Category);
            if (category != null)
            {
                var newCategory = _mapper.Map(model, category);
                await _context.SaveChangesAsync();
                var success = new CategoryResultVM
                {
                    Message = "index",
                    IsSuccess = true
                };
                return success;
            }
            else
            {
                var error = new CategoryResultVM
                {
                    Message = "No category found.",
                    IsSuccess = false
                };
                return error;
            }
        }
    }
}
