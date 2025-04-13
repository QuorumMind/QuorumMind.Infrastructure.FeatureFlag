namespace QuorumMind.Infrastructure.FeatureFlag.Core.Models;

public class FeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public List<string>? TargetUsers { get; set; }
    public List<string>? TargetRoles { get; set; }
    public List<string>? Regions { get; set; }
    public string Source { get; set; } = string.Empty;
}