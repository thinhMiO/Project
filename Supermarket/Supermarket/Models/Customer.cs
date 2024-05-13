using System;
using System.Collections.Generic;

namespace Supermarket.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public DateTime? CareateDate { get; set; }

    public bool? Active { get; set; }

    public bool? Gender { get; set; }

    public string? RandomKey { get; set; }

    public int? RoleId { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }
}
