using onlineshop.Models;

namespace onlineshop.Helper
{
    public static class DiscountExtention
    {
        public static long GetDiscount(this T_Product product)
        {
            return product.Price - (product.Price * product.Discount.Value / 100);
        }
    }
}
