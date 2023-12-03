using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Reaction
{
    public int ReactionId { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
