using NexCart.Domain.Stores;

namespace NexCart.Application.Features.Stores;

/// <summary>How a Store Handle problem is named to the client, both as a reason code and a message for the form.</summary>
public static class StoreHandleReasons
{
    public const string Taken = "taken";
    public const string TakenMessage = "This handle is already taken.";

    public static string ToReason(this StoreHandleProblem problem) => problem switch
    {
        StoreHandleProblem.Reserved => "reserved",
        _ => "invalid"
    };

    public static string ToMessage(this StoreHandleProblem problem) => problem switch
    {
        StoreHandleProblem.Reserved => "This handle is reserved by the platform.",
        _ => "Use 3 to 40 lowercase letters, digits and single hyphens, not starting or ending with a hyphen."
    };
}
