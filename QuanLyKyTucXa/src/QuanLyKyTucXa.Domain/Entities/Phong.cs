using System;
using System.Collections.Generic;

namespace QuanLyKyTucXa.Infrastructure;

public partial class Phong
{
    public string MaPhong { get; set; } = null!;

    public string MaToa { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public int Tang { get; set; }

    public string LoaiPhong { get; set; } = null!;

    public int SoChoToiDa { get; set; }

    public int SoNguoiHienTai { get; set; }

    public decimal GiaPhong { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiSoDienNuoc> ChiSoDienNuocs { get; set; } = new List<ChiSoDienNuoc>();

    public virtual ICollection<DangKyKtx> DangKyKtxes { get; set; } = new List<DangKyKtx>();

    public virtual ICollection<Giuong> Giuongs { get; set; } = new List<Giuong>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ToaNha MaToaNavigation { get; set; } = null!;

    public virtual ICollection<SuCo> SuCos { get; set; } = new List<SuCo>();
}
