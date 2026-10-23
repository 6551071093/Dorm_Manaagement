using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class HopDong
{
    public string MaHopDong { get; set; } = null!;

    public string MaSv { get; set; } = null!;

    public string MaGiuong { get; set; } = null!;

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public decimal TienCoc { get; set; }

    public decimal TienPhongThang { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual Giuong MaGiuongNavigation { get; set; } = null!;

    public virtual SinhVien MaSvNavigation { get; set; } = null!;
}
