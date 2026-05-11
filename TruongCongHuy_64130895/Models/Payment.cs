using System;
using System.Collections.Generic;

namespace TruongCongHuy_64130895.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual Order? Order { get; set; } = null!;
}
