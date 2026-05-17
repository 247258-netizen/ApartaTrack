using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Services;

public class UserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public Task<List<ApplicationUser>> GetAllAsync() =>
        _db.Users.OrderBy(u => u.FullName).ToListAsync();

    public Task<ApplicationUser?> GetByIdAsync(int id) =>
        _db.Users.FindAsync(id).AsTask();

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user, string plainPassword)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        user.CreatedAt = DateTime.Now;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ToggleActiveAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { _db.Users.Remove(user); await _db.SaveChangesAsync(); }
    }
}
