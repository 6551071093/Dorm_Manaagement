using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaHopDong { get; set; } = null!;

    public string MaSv { get; set; } = null!;

    public string MaPhong { get; set; } = null!;

    public int Thang { get; set; }

    public int Nam { get; set; }

    public decimal TongTien { get; set; }

    public DateOnly HanThanhToan { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual HopDong MaHopDongNavigation { get; set; } = null!;

    public virtual Phong MaPhongNavigation { get; set; } = null!;

    public virtual SinhVien MaSvNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
