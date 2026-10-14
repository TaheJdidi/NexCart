using FluentValidation;
using NexCart.Application.Interfaces;
using NexCart.Domain.Stores;

namespace NexCart.Application.Features.Stores.CreateStore;

public sealed class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator(IStoreRepository stores)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Handle).CustomAsync(async (input, context, cancellationToken) =>
        {
            var (handle, problem) = StoreHandle.Parse(input);

            if (handle is null)
            {
                context.AddFailure(problem!.Value.ToMessage());
            }
            // A handle with a problem need not be looked up
            else if (await stores.IsHandleTakenAsync(handle.Value, cancellationToken))
            {
                context.AddFailure(StoreHandleReasons.TakenMessage);
            }
        });

        // Omitted means the default currency
        RuleFor(x => x.Currency)
            .Must(Currencies.IsSupported)
            .When(x => x.Currency is not null)
            .WithMessage("Use an ISO 4217 currency code, such as TND or EUR.");
    }
}
