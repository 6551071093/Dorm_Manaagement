using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class VwBaoCaoDienNuoc
{
    public int MaChiSo { get; set; }

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public string MaToa { get; set; } = null!;

    public int Thang { get; set; }

    public int Nam { get; set; }

    public int ChiSoDienDau { get; set; }

    public int ChiSoDienCuoi { get; set; }

    public int? SoDienTieuThu { get; set; }

    public int ChiSoNuocDau { get; set; }

    public int ChiSoNuocCuoi { get; set; }

    public int? SoNuocTieuThu { get; set; }

    public DateOnly NgayGhi { get; set; }

    public string? NguoiGhi { get; set; }
}
