using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class VwCongNoHoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaHopDong { get; set; } = null!;

    public string MaSv { get; set; } = null!;

    public string TenSinhVien { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public string MaToa { get; set; } = null!;

    public int Thang { get; set; }

    public int Nam { get; set; }

    public decimal TongTien { get; set; }

    public decimal DaThanhToan { get; set; }

    public decimal? ConNo { get; set; }

    public DateOnly HanThanhToan { get; set; }

    public string TrangThai { get; set; } = null!;

    public int DaQuaHan { get; set; }
}
