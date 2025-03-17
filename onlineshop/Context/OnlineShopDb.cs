using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using onlineshop.Models;

namespace onlineshop.Context
{
    public class OnlineShopDb : IdentityDbContext<T_User>
    {
        public OnlineShopDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<T_Product> T_Product { get; set; }
        public DbSet<T_L_ProductPhoto> T_L_ProductPhoto { get; set; }
        public DbSet<T_L_Category> T_L_Category { get; set; }
        public DbSet<T_Basket> T_Basket { get; set; }
        public DbSet<T_Order> T_Order { get; set; }
        public DbSet<T_TempBasket> T_TempBasket { get; set; }
        public DbSet<T_TempOrder> T_TempOrder { get; set; }
        public DbSet<T_Payment> T_Payment { get; set; }

    }
}
