namespace onlineshop.Models
{
    public class ResponseVM
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public Object? Data { get; set; }
    }
}
