using MediatR;
using NexCart.Application.Interfaces;
using NexCart.Application.Mappings;

namespace NexCart.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IIdentityService identityService, IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.ValidateCredentialsAsync(request.Email.Trim(), request.Password, cancellationToken);

        return new AuthResult(user.ToResponse(), tokenGenerator.Generate(user));
    }
}
