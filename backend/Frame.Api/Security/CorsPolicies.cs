namespace Frame.Api.Security;

/// <summary>
/// Names of the CORS policies: which browser origins may call which routes.
/// The allowed origins are defined once in Program.cs, from configuration.
/// </summary>
internal static class CorsPolicies
{
    /// <summary>Default for customer routes: the customer site only (and Live Server in development).</summary>
    public const string Web = "WebApp";

    /// <summary>For /api/admin routes: the admin app only (and Live Server in development).</summary>
    public const string Admin = "AdminApp";
}
