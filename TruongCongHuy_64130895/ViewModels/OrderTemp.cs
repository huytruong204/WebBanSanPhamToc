namespace TruongCongHuy_64130895.ViewModels
{
    public class OrderTemp
    {
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
