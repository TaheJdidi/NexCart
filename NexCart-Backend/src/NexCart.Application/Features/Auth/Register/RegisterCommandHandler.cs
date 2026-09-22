using MediatR;
using NexCart.Application.Interfaces;
using NexCart.Application.Mappings;

namespace NexCart.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(IIdentityService identityService, IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.CreateUserAsync(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.Trim(),
            request.Password,
            cancellationToken);

        return new AuthResult(user.ToResponse(), tokenGenerator.Generate(user));
    }
}
