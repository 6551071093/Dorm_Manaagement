using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class VwTinhTrangPhongKtx
{
    public string MaToa { get; set; } = null!;

    public string TenToa { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public int Tang { get; set; }

    public string LoaiPhong { get; set; } = null!;

    public int SoChoToiDa { get; set; }

    public int SoNguoiHienTai { get; set; }

    public int? SoChoTrong { get; set; }

    public decimal GiaPhong { get; set; }

    public string TrangThai { get; set; } = null!;

    public decimal? TyLeLapDay { get; set; }
}
