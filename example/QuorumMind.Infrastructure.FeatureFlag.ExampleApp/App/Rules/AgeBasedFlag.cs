using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;


public class AgeBasedFlag : IFeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public string Source { get; set; } = "Custom:Age";

    public bool IsEnabled(FeatureFlagContext context)
    {
        if (context.Attributes?.TryGetValue("age", out var ageStr) == true && int.TryParse(ageStr, out var age))
            return age >= MinAge;
        return false;
    }
}