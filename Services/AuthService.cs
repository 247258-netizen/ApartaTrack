using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Services;

/// <summary>
/// Handles user authentication - login, logout, session management.
/// Scoped service — one instance per Blazor circuit (user session).
/// </summary>
public class AuthService
{
    private readonly AppDbContext _db;
    public ApplicationUser? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event Action? OnAuthStateChanged;

    public AuthService(AppDbContext db) => _db = db;

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) return false;

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!valid) return false;

        user.LastLogin = DateTime.Now;
        await _db.SaveChangesAsync();

        CurrentUser = user;
        OnAuthStateChanged?.Invoke();
        return true;
    }

    public void Logout()
    {
        CurrentUser = null;
        OnAuthStateChanged?.Invoke();
    }

    // ── Role checks ──────────────────────────────────────────────────────
    public bool IsAdmin()   => CurrentUser?.Role == UserRole.Admin;
    public bool IsManager() => CurrentUser?.Role is UserRole.Admin or UserRole.Manager;
    public bool IsStaff()   => IsLoggedIn;

    /// <summary>
    /// Returns true if the current user has at least the required role.
    /// Usage: Auth.HasRole(UserRole.Manager)
    /// </summary>
    public bool HasRole(UserRole required) => required switch
    {
        UserRole.Admin   => IsAdmin(),
        UserRole.Manager => IsManager(),
        UserRole.Staff   => IsStaff(),
        _                => false
    };

    /// <summary>
    /// Redirects to /login if not authenticated, or /unauthorized if wrong role.
    /// Call at top of OnInitializedAsync in protected pages.
    /// </summary>
    public bool RequireRole(NavigationManager nav, UserRole required = UserRole.Staff)
    {
        if (!IsLoggedIn)        { nav.NavigateTo("/login");        return false; }
        if (!HasRole(required)) { nav.NavigateTo("/unauthorized"); return false; }
        return true;
    }
}
