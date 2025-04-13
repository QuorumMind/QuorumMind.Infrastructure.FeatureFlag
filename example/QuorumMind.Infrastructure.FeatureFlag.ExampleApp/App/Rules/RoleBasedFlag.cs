using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;

public class RoleBasedFlag : IFeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<string> AllowedRoles { get; set; } = new();
    public string Source { get; set; } = "Custom:Role";

    public bool IsEnabled(FeatureFlagContext context)
    {
        if (context.Attributes?.TryGetValue("role", out var role) == true)
            return AllowedRoles.Contains(role);
        return false;
    }
}