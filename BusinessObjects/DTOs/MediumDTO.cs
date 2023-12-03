using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class MediumDTO
    {
        public int MediaId { get; set; }

        public int PostId { get; set; }

        public string MediaType { get; set; } = null!;

        public string MediaUrl { get; set; } = null!;
    }
}
