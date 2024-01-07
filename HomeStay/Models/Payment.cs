using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Bookings = new HashSet<Booking>();
        }

        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public int Card { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
