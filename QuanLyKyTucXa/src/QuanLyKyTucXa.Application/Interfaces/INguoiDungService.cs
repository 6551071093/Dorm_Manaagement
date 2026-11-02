using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.Application.Interfaces;

public interface INguoiDungService
{
    Task<List<UserAccountRow>> ListAsync();
    Task<UserAccountEdit> GetAsync(int id);
    Task<List<StudentAccountChoice>> StudentChoicesAsync(int? userId);
    Task SaveAsync(UserAccountEdit value);
    Task ToggleLockAsync(int id);
}
