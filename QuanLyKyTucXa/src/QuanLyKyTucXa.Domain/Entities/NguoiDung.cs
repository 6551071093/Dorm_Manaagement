using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string VaiTro { get; set; } = null!;

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? LanDangNhapCuoi { get; set; }

    public virtual ICollection<DangKyKtx> DangKyKtxes { get; set; } = new List<DangKyKtx>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
