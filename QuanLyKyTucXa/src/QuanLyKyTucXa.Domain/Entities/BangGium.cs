using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class BangGium
{
    public int MaBangGia { get; set; }

    public string TenDichVu { get; set; } = null!;

    public string DonViTinh { get; set; } = null!;

    public decimal DonGia { get; set; }

    public DateOnly NgayApDung { get; set; }

    public bool TrangThai { get; set; }
}
