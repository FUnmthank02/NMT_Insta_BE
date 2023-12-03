using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Notification
{
    public int NotifyId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public bool? IsRead { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; }

    public int? PostId { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? ToUser { get; set; }
}
