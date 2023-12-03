using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Medium
{
    public int MediaId { get; set; }

    public int PostId { get; set; }

    public string MediaType { get; set; } = null!;

    public string MediaUrl { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
