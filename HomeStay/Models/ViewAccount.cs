using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class ViewAccount
    {
        public int AccountId { get; set; }
        public int? RoleId { get; set; }
        public string AccountName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool Active { get; set; }
        public string Password { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string? RoleName { get; set; } = null!;

        public virtual Role? Role { get; set; }
    }
}