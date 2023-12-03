using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class UserUpdateDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Password { get; set; }

        public bool? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }
        public string? Avatar { get; set; }

        public bool? Status { get; set; }
    }
}
