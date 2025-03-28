namespace onlineshop.Models
{
    public class ProductResultVM
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T_Product? Product { get; set; }
        public List<T_Product>? Products { get; set; }
    }
}
