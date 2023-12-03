﻿using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class NotifyDTO
    {
        public int NotifyId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? FromUserId { get; set; }

        public int? ToUserId { get; set; }

        public bool? IsRead { get; set; }

        public string? Type { get; set; }

        public string? Content { get; set; }

        public int? PostId { get; set; }

        public virtual UserDTO? FromUser { get; set; }

        public virtual PostDTO? Post { get; set; }

        public virtual UserDTO? ToUser { get; set; }
    }
}
