using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class Giuong
{
    public string MaGiuong { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public string TenGiuong { get; set; } = null!;

    public string? ViTri { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual HopDong? HopDong { get; set; }

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
