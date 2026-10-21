using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure;

namespace QuanLyKyTucXa.Infrastructure.Data.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BangGium> BangGia { get; set; }

    public virtual DbSet<ChiSoDienNuoc> ChiSoDienNuocs { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DangKyKtx> DangKyKtxes { get; set; }

    public virtual DbSet<Giuong> Giuongs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<HopDong> HopDongs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<SuCo> SuCos { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<ToaNha> ToaNhas { get; set; }

    public virtual DbSet<VwBaoCaoDienNuoc> VwBaoCaoDienNuocs { get; set; }

    public virtual DbSet<VwCongNoHoaDon> VwCongNoHoaDons { get; set; }

    public virtual DbSet<VwDanhSachSinhVienNoTru> VwDanhSachSinhVienNoTrus { get; set; }

    public virtual DbSet<VwTinhTrangPhongKtx> VwTinhTrangPhongKtxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BangGium>(entity =>
        {
            entity.HasKey(e => e.MaBangGia);

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh).HasMaxLength(30);
            entity.Property(e => e.TenDichVu).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ChiSoDienNuoc>(entity =>
        {
            entity.HasKey(e => e.MaChiSo);

            entity.ToTable("ChiSoDienNuoc");

            entity.HasIndex(e => new { e.MaPhong, e.Thang, e.Nam }, "UQ_ChiSoDienNuoc_Phong_Thang_Nam").IsUnique();

            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.NgayGhi).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.NguoiGhi).HasMaxLength(100);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.ChiSoDienNuocs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_ChiSoDienNuoc_Phong");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet);

            entity.ToTable("ChiTietHoaDon", tb => tb.HasTrigger("TR_ChiTietHoaDon_CapNhatTongTien"));

            entity.HasIndex(e => e.MaHoaDon, "IX_ChiTietHoaDon_MaHoaDon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.LoaiTien).HasMaxLength(50);
            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.SoLuong)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHoaDon)
                .HasConstraintName("FK_ChiTietHoaDon_HoaDon");
        });

        modelBuilder.Entity<DangKyKtx>(entity =>
        {
            entity.HasKey(e => e.MaDangKy);

            entity.ToTable("DangKyKtx");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.HocKy).HasDefaultValue(1);
            entity.Property(e => e.MaPhongDeXuat).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.NamHoc).HasMaxLength(20);
            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ duyệt");

            entity.HasOne(d => d.MaPhongDeXuatNavigation).WithMany(p => p.DangKyKtxes)
                .HasForeignKey(d => d.MaPhongDeXuat)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DangKyKtx_Phong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.DangKyKtxes)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK_DangKyKtx_SinhVien");

            entity.HasOne(d => d.NguoiDuyetNavigation).WithMany(p => p.DangKyKtxes)
                .HasForeignKey(d => d.NguoiDuyet)
                .HasConstraintName("FK_DangKyKtx_NguoiDuyet");
        });

        modelBuilder.Entity<Giuong>(entity =>
        {
            entity.HasKey(e => e.MaGiuong);

            entity.ToTable("Giuong");

            entity.HasIndex(e => e.TrangThai, "IX_Giuong_TrangThai");

            entity.Property(e => e.MaGiuong).HasMaxLength(30);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.TenGiuong).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Trống");
            entity.Property(e => e.ViTri).HasMaxLength(30);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_Giuong_Phong");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon);

            entity.ToTable("HoaDon");

            entity.HasIndex(e => new { e.MaPhong, e.Thang, e.Nam }, "IX_HoaDon_MaPhong_Thang_Nam");

            entity.HasIndex(e => new { e.MaSv, e.TrangThai }, "IX_HoaDon_MaSV_TrangThai");

            entity.HasIndex(e => new { e.TrangThai, e.HanThanhToan }, "IX_HoaDon_TrangThai_HanThanhToan");

            entity.HasIndex(e => new { e.MaHopDong, e.Thang, e.Nam }, "UQ_HoaDon_HopDong_Thang_Nam").IsUnique();

            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaHopDong).HasMaxLength(30);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.MaHopDongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaHopDong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_HopDong");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_Phong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_SinhVien");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong);

            entity.ToTable("HopDong", tb => tb.HasTrigger("TR_HopDong_DongBoPhongGiuong"));

            entity.HasIndex(e => e.MaSv, "IX_HopDong_MaSV");

            entity.HasIndex(e => new { e.TrangThai, e.NgayKetThuc }, "IX_HopDong_TrangThai_NgayKetThuc");

            entity.HasIndex(e => e.MaGiuong, "UQ_HopDong_Giuong_HieuLuc")
                .IsUnique()
                .HasFilter("([TrangThai]=N'Hiệu lực')");

            entity.HasIndex(e => e.MaSv, "UQ_HopDong_SinhVien_HieuLuc")
                .IsUnique()
                .HasFilter("([TrangThai]=N'Hiệu lực')");

            entity.Property(e => e.MaHopDong).HasMaxLength(30);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaGiuong).HasMaxLength(30);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TienCoc)
                .HasDefaultValue(500000m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienPhongThang).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Hiệu lực");

            entity.HasOne(d => d.MaGiuongNavigation).WithOne(p => p.HopDong)
                .HasForeignKey<HopDong>(d => d.MaGiuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HopDong_Giuong");

            entity.HasOne(d => d.MaSvNavigation).WithOne(p => p.HopDong)
                .HasForeignKey<HopDong>(d => d.MaSv)
                .HasConstraintName("FK_HopDong_SinhVien");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung);

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ_NguoiDung_TenDangNhap").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValue("NhanVien");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong);

            entity.ToTable("Phong");

            entity.HasIndex(e => new { e.MaToa, e.Tang }, "IX_Phong_MaToa_Tang");

            entity.HasIndex(e => e.TrangThai, "IX_Phong_TrangThai");

            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GiaPhong)
                .HasDefaultValue(500000m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiPhong)
                .HasMaxLength(50)
                .HasDefaultValue("Phòng 4 người");
            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.SoChoToiDa).HasDefaultValue(4);
            entity.Property(e => e.Tang).HasDefaultValue(1);
            entity.Property(e => e.TenPhong).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Còn chỗ");

            entity.HasOne(d => d.MaToaNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaToa)
                .HasConstraintName("FK_Phong_ToaNha");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv);

            entity.ToTable("SinhVien");

            entity.HasIndex(e => e.HoTen, "IX_SinhVien_HoTen");

            entity.HasIndex(e => new { e.Lop, e.Khoa }, "IX_SinhVien_Lop_Khoa");

            entity.HasIndex(e => e.TrangThai, "IX_SinhVien_TrangThai");

            entity.HasIndex(e => e.Cccd, "UQ_SinhVien_CCCD").IsUnique();

            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Khoa).HasMaxLength(100);
            entity.Property(e => e.Lop).HasMaxLength(50);
            entity.Property(e => e.QueQuan).HasMaxLength(200);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ xếp phòng");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SinhVien_NguoiDung");
        });

        modelBuilder.Entity<SuCo>(entity =>
        {
            entity.HasKey(e => e.MaSuCo);

            entity.ToTable("SuCo");

            entity.HasIndex(e => e.MaPhong, "IX_SuCo_MaPhong");

            entity.HasIndex(e => e.TrangThai, "IX_SuCo_TrangThai");

            entity.Property(e => e.ChiPhiSuaChua).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.MucDoUuTien)
                .HasMaxLength(20)
                .HasDefaultValue("Bình thường");
            entity.Property(e => e.NgayBao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NguoiXuLy).HasMaxLength(100);
            entity.Property(e => e.TieuDe).HasMaxLength(200);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Mới tiếp nhận");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.SuCos)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_SuCo_Phong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.SuCos)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SuCo_SinhVien");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan);

            entity.ToTable("ThanhToan", tb => tb.HasTrigger("TR_ThanhToan_CapNhatTrangThaiHoaDon"));

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NguoiThu).HasMaxLength(100);
            entity.Property(e => e.PhuongThuc)
                .HasMaxLength(50)
                .HasDefaultValue("Chuyển khoản VietQR");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaHoaDon)
                .HasConstraintName("FK_ThanhToan_HoaDon");
        });

        modelBuilder.Entity<ToaNha>(entity =>
        {
            entity.HasKey(e => e.MaToa);

            entity.ToTable("ToaNha");

            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.LoaiToa)
                .HasMaxLength(20)
                .HasDefaultValue("Hỗn hợp");
            entity.Property(e => e.SoTang).HasDefaultValue(5);
            entity.Property(e => e.TenToa).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Hoạt động");
        });

        modelBuilder.Entity<VwBaoCaoDienNuoc>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BaoCaoDienNuoc");

            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.NguoiGhi).HasMaxLength(100);
            entity.Property(e => e.TenPhong).HasMaxLength(50);
        });

        modelBuilder.Entity<VwCongNoHoaDon>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CongNoHoaDon");

            entity.Property(e => e.ConNo).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.DaThanhToan).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.MaHopDong).HasMaxLength(30);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenPhong).HasMaxLength(50);
            entity.Property(e => e.TenSinhVien).HasMaxLength(100);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(30);
        });

        modelBuilder.Entity<VwDanhSachSinhVienNoTru>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_DanhSachSinhVienNoTru");

            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Khoa).HasMaxLength(100);
            entity.Property(e => e.Lop).HasMaxLength(50);
            entity.Property(e => e.MaGiuong).HasMaxLength(30);
            entity.Property(e => e.MaHopDong).HasMaxLength(30);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(20)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.QueQuan).HasMaxLength(200);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenGiuong).HasMaxLength(50);
            entity.Property(e => e.TenPhong).HasMaxLength(50);
            entity.Property(e => e.TenToa).HasMaxLength(100);
        });

        modelBuilder.Entity<VwTinhTrangPhongKtx>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TinhTrangPhongKTX");

            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiPhong).HasMaxLength(50);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.MaToa).HasMaxLength(10);
            entity.Property(e => e.TenPhong).HasMaxLength(50);
            entity.Property(e => e.TenToa).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(30);
            entity.Property(e => e.TyLeLapDay).HasColumnType("decimal(5, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
