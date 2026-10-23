using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class ChiTietHoaDon
{
    public int MaChiTiet { get; set; }

    public string MaHoaDon { get; set; } = null!;

    public string LoaiTien { get; set; } = null!;

    public decimal SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }

    public string? GhiChu { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
}
