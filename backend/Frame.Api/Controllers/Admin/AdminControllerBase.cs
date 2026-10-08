using Frame.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Frame.Api.Controllers.Admin;

/// <summary>
/// Base class for EVERY admin controller. Inheriting from it means:
/// admin role + admin-audience token required (secure by default),
/// and only the admin app's origin may call it from a browser.
/// A new admin controller cannot forget the protection: it comes with the class.
/// </summary>
[ApiController]
[Authorize(Policy = AuthPolicies.Admin)]
[EnableCors(CorsPolicies.Admin)]
public abstract class AdminControllerBase : ControllerBase
{
}
