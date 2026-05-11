namespace TruongCongHuy_64130895.Models
{
    public class MENU
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string? MenuUrl { get; set; }
        public int MenuIndex { get; set; }
        public bool IsVisible { get; set; }
    }
}
