using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class FollowerCreateDTO
    {
        public int FollowerId { get; set; }

        public int FollowingId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
