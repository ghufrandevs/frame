using Frame.Application.Auth.Dtos;

namespace Frame.Application.Auth;

/// <summary>
/// Sign-up, login and profile for customers and admins.
/// Controllers depend on this interface, never on the class.
/// </summary>
public interface IAuthService
{
    /// <summary>Creates a Customer account and logs it in. Throws EMAIL_TAKEN if the email is used.</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Customer login. Admin accounts are refused with INVALID_CREDENTIALS.</summary>
    Task<AuthResponse> LoginCustomerAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>Admin login for the separate admin app. Customer accounts are refused with INVALID_CREDENTIALS.</summary>
    Task<AuthResponse> LoginAdminAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>The logged-in user's profile, from the id inside their token.</summary>
    Task<ProfileResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>Changes the name and phone. The email (login name) never changes.</summary>
    Task<ProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}