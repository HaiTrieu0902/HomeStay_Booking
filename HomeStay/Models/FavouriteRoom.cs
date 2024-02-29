using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class FavouriteRoom
    {
        public int FavouriteRoomId { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public string Title { get; set; } = null!;
        public double Price { get; set; }
        public string Detail { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string Avatar { get; set; } = null!;

        public virtual Customer Customer { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}
