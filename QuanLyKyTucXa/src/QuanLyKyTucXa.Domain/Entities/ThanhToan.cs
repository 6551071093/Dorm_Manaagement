using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class ThanhToan
{
    public int MaThanhToan { get; set; }

    public string MaHoaDon { get; set; } = null!;

    public DateTime NgayThanhToan { get; set; }

    public decimal SoTien { get; set; }

    public string PhuongThuc { get; set; } = null!;

    public string? MaGiaoDich { get; set; }

    public string? NguoiThu { get; set; }

    public string? GhiChu { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
