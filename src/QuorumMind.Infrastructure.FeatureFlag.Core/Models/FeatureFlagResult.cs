namespace QuorumMind.Infrastructure.FeatureFlag.Core.Models;


public class FeatureFlagResult
{
    public bool? IsEnabled { get; set; }
    public string Source { get; set; } = string.Empty;
}