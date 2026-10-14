using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using NexCart.Application.Features.Stores.CheckHandleAvailability;
using NexCart.Application.Features.Stores.CreateStore;
using NexCart.Contracts.Stores;

namespace NexCart.API.Controllers;

[ApiController]
[Route("api/stores")]
public sealed class StoresController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType<StoreResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoreResponse>> Create(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var accountId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new UnauthorizedAccessException("The token has no subject.");

        var store = await sender.Send(
            new CreateStoreCommand(accountId, request.Name, request.Handle, request.Currency),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, store);
    }

    // Anonymous so the Create a Store form can check the handle as the Merchant types
    [HttpGet("handle-availability")]
    [ProducesResponseType<HandleAvailabilityResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<HandleAvailabilityResponse>> CheckHandleAvailability(
        [FromQuery] string handle, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new CheckHandleAvailabilityQuery(handle), cancellationToken));
}
