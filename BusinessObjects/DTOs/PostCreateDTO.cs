using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class PostCreateDTO
    {
        public string? Caption { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int UserId { get; set; }

    }
}
 