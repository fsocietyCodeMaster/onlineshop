using onlineshop.Repositroy;
using onlineshop.Services;

namespace onlineshop.Helper
{
    public static class ServiceExtention
    {
        public static IServiceCollection AddCustomService (this IServiceCollection services)
        {
            services.AddScoped<IUser,UserService>();
            services.AddScoped<ICategory,CategoryService>();
            services.AddScoped<IProduct,ProductService>();
            services.AddScoped<DataSeeder>();
            services.AddScoped<IOnlineShop, OnlineShopService>();
            return services;
        }
    }
}
