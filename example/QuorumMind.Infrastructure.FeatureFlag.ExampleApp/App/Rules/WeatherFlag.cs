using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;

public class WeatherFlag : IFeatureFlagDefinition
{
    public string Name { get; set; } = string.Empty;
    public double MaxTemperature { get; set; }
    public string Source { get; set; } = "Custom:Weather";

    public bool IsEnabled(FeatureFlagContext context)
    {
        if (context.Attributes?.TryGetValue("temp", out var tempStr) == true && double.TryParse(tempStr, out var temp))
            return temp <= MaxTemperature;
        return false;
    }
}