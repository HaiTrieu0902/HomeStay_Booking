using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Sach
    {
        public int PkSachId { get; set; }
        public string? TenSach { get; set; }
        public string? TacGia { get; set; }
        public int? NamXuatBan { get; set; }
        public int? SoLuong { get; set; }
        public double? Gia { get; set; }
    }
}
