using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;

public class UserTargetedFlag : IFeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<string> UserIds { get; set; } = new();
    public string Source { get; set; } = "Custom:User";

    public bool IsEnabled(FeatureFlagContext context)
    {
        return context.UserId != null && UserIds.Contains(context.UserId);
    }
}