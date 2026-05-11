namespace TruongCongHuy_64130895.Areas.Admin.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
