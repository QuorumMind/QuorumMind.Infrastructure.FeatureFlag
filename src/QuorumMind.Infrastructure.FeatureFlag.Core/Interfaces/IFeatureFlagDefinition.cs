using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;


public interface IFeatureFlagDefinition
{
    string Name { get; }
    bool IsEnabled(FeatureFlagContext context);
    string Source { get; }
}