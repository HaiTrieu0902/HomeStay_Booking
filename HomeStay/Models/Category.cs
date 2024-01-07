using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Category
    {
        public Category()
        {
            Rooms = new HashSet<Room>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
