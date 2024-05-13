using System;
using System.Collections.Generic;

namespace Supermarket.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public double? Discount { get; set; }

    public int? CategoryId { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public bool? BestSeller { get; set; }

    public bool? HomeFlag { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
