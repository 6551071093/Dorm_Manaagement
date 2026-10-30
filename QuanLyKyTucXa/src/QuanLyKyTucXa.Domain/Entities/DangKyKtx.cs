using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class DangKyKtx
{
    public int MaDangKy { get; set; }

    public string MaSv { get; set; } = null!;

    public string? MaPhongDeXuat { get; set; }

    public int HocKy { get; set; }

    public string NamHoc { get; set; } = null!;

    public DateTime NgayDangKy { get; set; }

    public string TrangThai { get; set; } = null!;

    public int? NguoiDuyet { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public string? GhiChu { get; set; }

    public virtual Phong? MaPhongDeXuatNavigation { get; set; }

    public virtual SinhVien MaSvNavigation { get; set; } = null!;

    public virtual NguoiDung? NguoiDuyetNavigation { get; set; }
}
