using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class SuCo
{
    public int MaSuCo { get; set; }

    public string MaPhong { get; set; } = null!;

    public string? MaSv { get; set; }

    public string TieuDe { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public string MucDoUuTien { get; set; } = null!;

    public DateTime NgayBao { get; set; }

    public DateTime? NgayXuLy { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? NguoiXuLy { get; set; }

    public decimal ChiPhiSuaChua { get; set; }

    public virtual Phong MaPhongNavigation { get; set; } = null!;

    public virtual SinhVien? MaSvNavigation { get; set; }
}
