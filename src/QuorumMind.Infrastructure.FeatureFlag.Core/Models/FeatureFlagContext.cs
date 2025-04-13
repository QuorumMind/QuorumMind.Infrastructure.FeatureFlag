namespace QuorumMind.Infrastructure.FeatureFlag.Core.Models;

public class FeatureFlagContext
{
    public string FlagName { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}