using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;

public class RegionBasedFlag : IFeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<string> AllowedRegions { get; set; } = new();
    public string Source { get; set; } = "Custom:Region";

    public bool IsEnabled(FeatureFlagContext context)
    {
        if (context.Attributes?.TryGetValue("region", out var region) == true)
            return AllowedRegions.Contains(region);
        return false;
    }
}
