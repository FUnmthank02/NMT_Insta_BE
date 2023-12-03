using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class ReactionCreateDTO
    {

        public int UserId { get; set; }

        public int PostId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
