using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface INotificationService
{
    Task<List<Notification>> GetAllAsync();
    Task<List<Notification>> GetUnreadAsync();
    Task MarkReadAsync(int id);
    Task MarkAllReadAsync();
    Task DeleteAsync(int id);
    Task<int> GetUnreadCountAsync();
    Task GenerateAlertsAsync(); // Auto-generate payment/lease alerts
}
