using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class RoomImagesDetail
    {
        public int RoomId { get; set; }
        public string Images { get; set; } = null!;
        public string Type { get; set; } = null!;

        public virtual Room Room { get; set; } = null!;
    }
}
