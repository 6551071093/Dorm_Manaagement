using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string GioiTinh { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? Email { get; set; }

    public string Lop { get; set; } = null!;

    public string Khoa { get; set; } = null!;

    public string? QueQuan { get; set; }

    public string TrangThai { get; set; } = null!;

    public int? MaNguoiDung { get; set; }

    public virtual ICollection<DangKyKtx> DangKyKtxes { get; set; } = new List<DangKyKtx>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual HopDong? HopDong { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual ICollection<SuCo> SuCos { get; set; } = new List<SuCo>();
}
