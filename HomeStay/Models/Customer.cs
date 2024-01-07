using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Bookings = new HashSet<Booking>();
        }

        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Avatar { get; set; } = null!;
        public string? Address { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool Active { get; set; }
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
