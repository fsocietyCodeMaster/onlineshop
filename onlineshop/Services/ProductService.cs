using AutoMapper;
using Microsoft.EntityFrameworkCore;
using onlineshop.Context;
using onlineshop.Models;
using onlineshop.Repositroy;
using System.IO;

namespace onlineshop.Services
{
    public class ProductService : IProduct
    {
        private readonly OnlineShopDb _context;
        private readonly IMapper _mapper;

        public ProductService(OnlineShopDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ProductResultVM> CreateProduct(Create_ProductVM model)
        {
            if (model != null)
            {
                if (model.ImageUrl == null || !model.ImageUrl.Any())
                {
                    var error = new ProductResultVM
                    {
                        Message = "No file uploaded.",
                        IsSuccess = false
                    };
                    return error;
                }
                var product = _mapper.Map<T_Product>(model);
                _context.Add(product);
                await _context.SaveChangesAsync();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var uploadImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadImage))
                {
                    Directory.CreateDirectory(uploadImage);
                }
                var imageList = new List<T_L_ProductPhoto>();
                foreach (var file in model.ImageUrl)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        var error = new ProductResultVM
                        {
                            Message = $"Invalid file format: {file.FileName}. Allowed formats: {string.Join(", ", allowedExtensions)}",
                            IsSuccess = false
                        };
                        return error;
                    }
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    string url = Path.Combine(uploadImage, fileName);
                    using (var fileSystem = new FileStream(url, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSystem);
                    }
                    var image = new T_L_ProductPhoto
                    {
                        T_Product_ID = product.ID_Product,
                        ImageUrl = Path.Combine("images", fileName)
                    };
                    imageList.Add(image);
                }

                _context.AddRange(imageList); // this is the best way of adding list in db.
                await _context.SaveChangesAsync();
                var success = new ProductResultVM
                {
                    Message = "index",
                    IsSuccess = true

                };
                return success;

            }
            else
            {
                var error = new ProductResultVM
                {
                    Message = "There is problem sending the data.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<ProductResultVM> DeleteProduct(Guid id)
        {
            var product = await GetProductById(id);
            if (product != null)
            {
                var finalResult = product.Product;
                foreach (var photos in finalResult.Photos)
                {
                    var path = (Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photos.ImageUrl)); // maybe path doesnt exist 
                    File.Delete(path);
                    _context.Remove(photos);


                }
                _context.Remove(finalResult);
                await _context.SaveChangesAsync();
                var success = new ProductResultVM
                {
                    Message = "index",
                    IsSuccess = true
                };
                return success;
            }
            else
            {
                var error = new ProductResultVM
                {
                    Message = "There is no product.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<ProductResultVM> GetAllProducts()
        {
            var products = await _context.T_Product.Include(c => c.Category).Include(c => c.Photos).ToListAsync();
            if (products.Any())
            {
                var success = new ProductResultVM
                {
                    IsSuccess = true,
                    Products = products
                };
                return success;
            }
            else
            {
                var error = new ProductResultVM
                {
                    Message = "There is no product.",
                    IsSuccess = false
                };
                return error;
            }
        }

        public async Task<ProductResultVM> GetProductById(Guid id)
        {
            var product = await _context.T_Product.Include(c => c.Category).Include(c => c.Photos).FirstOrDefaultAsync(c => c.ID_Product == id);
            if (product != null)
            {
                var success = new ProductResultVM
                {
                    IsSuccess = true,
                    Product = product
                };
                return success;
            }
            else
            {
                var error = new ProductResultVM
                {
                    Message = "There is no product.",
                    IsSuccess = false
                };
                return error;
            }

        }

        public async Task<ProductResultVM> UpdateProduct(Guid id, Update_ProductVM model)
        {
            var product = await _context.T_Product.Include(c => c.Category).Include(c => c.Photos).FirstOrDefaultAsync(c => c.ID_Product == id);
            if (product != null)
            {
                var newProduct = _mapper.Map(model, product);
                var imageList = new List<T_L_ProductPhoto>();
                if (model.ImageUrl != null && model.ImageUrl.Any())
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var uploadImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadImage))
                    {
                        Directory.CreateDirectory(uploadImage);
                    }
                    foreach (var file in model.ImageUrl)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            var error = new ResponseVM
                            {
                                Message = $"Invalid file format: {file.FileName}. Allowed formats: {string.Join(", ", allowedExtensions)}",
                                IsSuccess = false
                            };
                        }
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        string url = Path.Combine(uploadImage, fileName);
                        using (var fileSystem = new FileStream(url, FileMode.Create))
                        {
                            await file.CopyToAsync(fileSystem);
                        }
                        var image = new T_L_ProductPhoto
                        {
                            T_Product_ID = product.ID_Product,
                            ImageUrl = Path.Combine("images", fileName)
                        };
                        imageList.Add(image);
                    }
                }
                _context.T_L_ProductPhoto.AddRange(imageList);
                await _context.SaveChangesAsync();
                var success = new ProductResultVM
                {
                    Message = "index",
                    IsSuccess = true
                };
                return success;
            }
            else
            {
                var error = new ProductResultVM
                {
                    Message = "No product found.",
                    IsSuccess = false
                };
                return error;
            }
        }
    }
}
