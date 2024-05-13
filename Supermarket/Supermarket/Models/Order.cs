using System;
using System.Collections.Generic;

namespace Supermarket.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? TransactStatusId { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? CustomerName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public double? TransportFee { get; set; }

    public string? PaymentMethod { get; set; }

    public double? TotalAmount { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual TransactStatus? TransactStatus { get; set; }
}
