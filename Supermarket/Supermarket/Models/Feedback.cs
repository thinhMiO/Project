using System;
using System.Collections.Generic;

namespace Supermarket.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }
}
