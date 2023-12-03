using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class PostDTO
    {
        public int PostId { get; set; }

        public string? Caption { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int UserId { get; set; }
        public virtual UserDTO User { get; set; } = null!;
        public virtual ICollection<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

        public virtual ICollection<MediumDTO> Media { get; set; } = new List<MediumDTO>();

        public virtual ICollection<ReactionDTO> Reactions { get; set; } = new List<ReactionDTO>();


    }
}
