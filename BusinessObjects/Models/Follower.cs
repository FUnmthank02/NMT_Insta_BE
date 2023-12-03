using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Follower
{
    public int FollowId { get; set; }

    public int FollowerId { get; set; }

    public int FollowingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User FollowerNavigation { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
