namespace QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsFeatureEnabledAsync(string featureName, string? userId = null, Dictionary<string, string>? attributes = null, CancellationToken cancellationToken = default);
}
