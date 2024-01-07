using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int? RoomId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int CountNight { get; set; }
        public double TotalAmount { get; set; }
        public int AmountOfPeople { get; set; }
        public int PaymentId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Payment Payment { get; set; } = null!;
        public virtual Room? Room { get; set; }
    }
}
