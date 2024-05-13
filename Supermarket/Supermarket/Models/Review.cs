using System;
using System.Collections.Generic;

namespace Supermarket.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public string? Contents { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
