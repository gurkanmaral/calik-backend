namespace CalikBackend.API.Configuration;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";
    public PolicyOptions GlobalPolicy { get; set; } = new();
    public PolicyOptions AuthPolicy   { get; set; } = new();
}

public sealed class PolicyOptions
{
    public int PermitLimit   { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
}
