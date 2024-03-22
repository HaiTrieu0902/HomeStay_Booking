using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStay.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
            FavouriteRooms = new HashSet<FavouriteRoom>();
        }

        public int RoomId { get; set; }
        public string Title { get; set; } = null!;
        public string Detail { get; set; } = null!;
        public double? Price { get; set; }
        public string Area { get; set; } = null!;
        public int Capacity { get; set; }
        public string Description { get; set; } = null!;
        public bool Active { get; set; }
        public string Status { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public int CategoryId { get; set; }
        [NotMapped]
        public string? CategoryName { get; set; }


        public virtual Category Category { get; set; } = null!;
        public virtual RoomImagesDetail? RoomImagesDetail { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<FavouriteRoom> FavouriteRooms { get; set; }
    }
}
