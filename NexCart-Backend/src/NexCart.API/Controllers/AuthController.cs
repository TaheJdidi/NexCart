using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using NexCart.Application.DTOs;
using NexCart.Application.Features.Auth.GetCurrentUser;
using NexCart.Application.Features.Auth.Login;
using NexCart.Application.Features.Auth.Register;
using NexCart.Contracts.Auth;
using NexCart.Infrastructure.Authentication;

namespace NexCart.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.FirstName, request.LastName, request.Email, request.Password),
            cancellationToken);

        // Signing up also signs the user in
        SetAuthCookie(result.Token);
        return CreatedAtAction(nameof(Me), result.User);
    }

    [HttpPost("login")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

        SetAuthCookie(result.Token);
        return Ok(result.User);
    }

    // Anonymous so that a user with an expired token can still clear the cookie
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookie.Name, AuthCookie.CreateOptions());
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new UnauthorizedAccessException("The token has no subject.");

        return Ok(await sender.Send(new GetCurrentUserQuery(userId), cancellationToken));
    }

    private void SetAuthCookie(AuthToken token) =>
        Response.Cookies.Append(AuthCookie.Name, token.Value, AuthCookie.CreateOptions(token.ExpiresAtUtc));
}
