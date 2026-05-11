using System;
using System.Collections.Generic;

namespace TruongCongHuy_64130895.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public int OrderId { get; set; }
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product? Product { get; set; } = null!;
    public virtual Order? Order { get; set; } = null!;
}
