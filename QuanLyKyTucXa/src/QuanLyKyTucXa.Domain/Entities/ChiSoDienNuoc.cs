using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class ChiSoDienNuoc
{
    public int MaChiSo { get; set; }

    public string MaPhong { get; set; } = null!;

    public int Thang { get; set; }

    public int Nam { get; set; }

    public int ChiSoDienDau { get; set; }

    public int ChiSoDienCuoi { get; set; }

    public int ChiSoNuocDau { get; set; }

    public int ChiSoNuocCuoi { get; set; }

    public DateOnly NgayGhi { get; set; }

    public string? NguoiGhi { get; set; }

    public virtual Phong MaPhongNavigation { get; set; } = null!;
}
