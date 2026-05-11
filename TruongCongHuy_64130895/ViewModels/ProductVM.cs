namespace TruongCongHuy_64130895.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price  { get; set; }
        public bool IsOnSale { get; set; }
        public decimal PriceSales { get; set; }

        public int Ratings { get; set; }
        public string Img { get; set; }
        public string CategoryName { get; set; }
    }
}
