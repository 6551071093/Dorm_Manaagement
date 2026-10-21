using Microsoft.EntityFrameworkCore;

namespace QuanLyKyTucXa.Infrastructure.Data.Context;

public sealed class KtxDbContext(DbContextOptions<KtxDbContext> options) : DbContext(options)
{
}
