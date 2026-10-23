using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class ToaNha
{
    public string MaToa { get; set; } = null!;

    public string TenToa { get; set; } = null!;

    public string LoaiToa { get; set; } = null!;

    public int SoTang { get; set; }

    public int SoPhong { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
