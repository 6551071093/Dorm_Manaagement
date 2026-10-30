using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class VwDanhSachSinhVienNoTru
{
    public string MaSv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string GioiTinh { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? Email { get; set; }

    public string Lop { get; set; } = null!;

    public string Khoa { get; set; } = null!;

    public string? QueQuan { get; set; }

    public string MaHopDong { get; set; } = null!;

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public string MaGiuong { get; set; } = null!;

    public string TenGiuong { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public int Tang { get; set; }

    public string MaToa { get; set; } = null!;

    public string TenToa { get; set; } = null!;
}
