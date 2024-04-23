using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Rating
    {
        public int RatingId { get; set; }
        public int? CustomerId { get; set; }
        public int? BookingId { get; set; }
        public int Rating1 { get; set; }
        public string Comment { get; set; } = null!;

        public virtual Booking? Booking { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
