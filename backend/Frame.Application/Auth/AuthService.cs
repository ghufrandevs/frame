using Frame.Application.Auth.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Frame.Application.Auth;

/// <summary>
/// Sign-up, login and profile. Requests reach here already validated (FluentValidation);
/// this class applies the business rules and talks to the abstractions only,
/// never to EF Core, JWT libraries or hashing code directly.
/// </summary>
internal sealed class AuthService : IAuthService
{
    private const string OmanCountryCode = "+968";

    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        ILogger<AuthService> logger)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = User.NormalizeEmail(request.Email);

        if (await _users.EmailExistsAsync(email, cancellationToken))
            throw AppException.Conflict(ErrorCodes.EmailTaken);

        var user = User.CreateCustomer(
            request.FullName.Trim(),
            email,
            NormalizePhone(request.Phone),
            _passwordHasher.Hash(request.Password));

        _users.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer registered {UserId}", user.Id);
        return BuildResponse(user);
    }

    public Task<AuthResponse> LoginCustomerAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => LoginAsync(request, UserRole.Customer, cancellationToken);

    public Task<AuthResponse> LoginAdminAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => LoginAsync(request, UserRole.Admin, cancellationToken);

    public async Task<ProfileResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(userId, cancellationToken);
        return ToProfile(user);
    }

    public async Task<ProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(userId, cancellationToken);

        user.UpdateProfile(request.FullName.Trim(), NormalizePhone(request.Phone));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated {UserId}", user.Id);
        return ToProfile(user);
    }

    /// <summary>
    /// Shared login logic. Unknown email, wrong password and wrong app
    /// all give the SAME answer, so the response never reveals which accounts exist.
    /// </summary>
    private async Task<AuthResponse> LoginAsync(LoginRequest request, UserRole allowedRole, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(request.Email);
        var user = await _users.GetByEmailAsync(email, cancellationToken);

        var isValid = user is not null
                      && user.Role == allowedRole
                      && _passwordHasher.Verify(user.PasswordHash, request.Password);

        if (!isValid)
        {
            // Never log the password or the email typed: only that an attempt failed.
            _logger.LogWarning("Failed {Role} login attempt", allowedRole);
            throw AppException.Unauthorized(ErrorCodes.InvalidCredentials);
        }

        _logger.LogInformation("{Role} logged in {UserId}", allowedRole, user!.Id);
        return BuildResponse(user);
    }

    /// <summary>A valid token for a user that no longer exists is treated as not logged in.</summary>
    private async Task<User> GetUserOrThrowAsync(int userId, CancellationToken cancellationToken)
        => await _users.GetByIdAsync(userId, cancellationToken)
           ?? throw AppException.Unauthorized();

    private AuthResponse BuildResponse(User user)
    {
        var token = _tokenGenerator.Generate(user);
        return new AuthResponse(token.Value, token.ExpiresAt, ToDto(user));
    }

    private static AuthUserDto ToDto(User user)
        => new(user.Id, user.FullName, user.Email, user.Role.ToString());

    private static ProfileResponse ToProfile(User user)
        => new(user.Id, user.FullName, user.Email, user.Phone, user.Role.ToString());

    /// <summary>Stores every phone the same way (8 digits), with or without +968.</summary>
    private static string NormalizePhone(string phone)
    {
        var trimmed = phone.Trim();
        return trimmed.StartsWith(OmanCountryCode) ? trimmed[OmanCountryCode.Length..] : trimmed;
    }
}