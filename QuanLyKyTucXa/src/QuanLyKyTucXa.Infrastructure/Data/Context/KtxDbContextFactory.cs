using Microsoft.EntityFrameworkCore;

namespace QuanLyKyTucXa.Infrastructure.Data.Context;

public static class KtxDbContextFactory
{
    public static KtxDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<KtxDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new KtxDbContext(options);
    }
}
