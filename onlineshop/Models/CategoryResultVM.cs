namespace onlineshop.Models
{
    public class CategoryResultVM
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T_L_Category? Category { get; set; }
        public List<T_L_Category>? Categories { get; set; }
    }
}
