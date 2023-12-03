using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class CommentDTO
    {
        public int CommentId { get; set; }

        public string Content { get; set; } = null!;

        public int PostId { get; set; }

        public int UserId { get; set; }

        public int? ParentCommentId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserDTO User { get; set; } = null!;
    }
}
