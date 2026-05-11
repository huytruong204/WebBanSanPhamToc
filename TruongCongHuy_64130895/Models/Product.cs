using System;
using System.Collections.Generic;

namespace TruongCongHuy_64130895.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsHot { get; set; }

    public bool IsOnSale { get; set; }

    public decimal? DiscountPercent { get; set; }
    public decimal? PriceSales
    {
        get; set;
    }
    public virtual Category? Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
