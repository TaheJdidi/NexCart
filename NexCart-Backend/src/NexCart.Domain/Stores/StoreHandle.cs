using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace NexCart.Domain.Stores;

/// <summary>
/// The unique, permanent short name of a Store that forms its subdomain. See CONTEXT.md.
/// Only the rules that need no other Store are checked here; whether a handle is taken is a lookup.
/// </summary>
public sealed partial record StoreHandle
{
    public const int MinimumLength = 3;
    public const int MaximumLength = 40;

    // Subdomains the platform uses or may use itself, so no Store can claim them
    private static readonly FrozenSet<string> Reserved = FrozenSet.ToFrozenSet(
    [
        "www", "api", "admin", "app", "platform", "mail", "static", "assets",
        "cdn", "media", "files", "img", "images", "smtp", "imap", "pop", "ftp",
        "auth", "login", "logout", "register", "signup", "account", "accounts",
        "dashboard", "help", "support", "status", "docs", "blog", "news",
        "dev", "staging", "test", "demo", "health", "store", "stores", "shop", "nexcart"
    ]);

    private StoreHandle(string value) => Value = value;

    public string Value { get; }

    public static StoreHandleParseResult Parse(string? input)
    {
        // Uniqueness is case-insensitive, so every handle is kept in one canonical lowercase form
        var candidate = (input ?? string.Empty).Trim().ToLowerInvariant();

        if (candidate.Length is < MinimumLength or > MaximumLength || !Format().IsMatch(candidate))
        {
            return new StoreHandleParseResult(null, StoreHandleProblem.Invalid);
        }

        if (Reserved.Contains(candidate))
        {
            return new StoreHandleParseResult(null, StoreHandleProblem.Reserved);
        }

        return new StoreHandleParseResult(new StoreHandle(candidate), null);
    }

    public override string ToString() => Value;

    // Lowercase letters and digits in groups joined by single hyphens, so no leading, trailing or double hyphen
    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex Format();
}

/// <summary>Exactly one of Handle and Problem is set.</summary>
public sealed record StoreHandleParseResult(StoreHandle? Handle, StoreHandleProblem? Problem);
