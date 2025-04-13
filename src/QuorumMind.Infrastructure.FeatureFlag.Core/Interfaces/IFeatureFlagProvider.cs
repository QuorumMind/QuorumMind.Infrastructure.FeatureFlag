using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

public interface IFeatureFlagProvider
{
    void RegisterKnownTypes(IEnumerable<Type> knownTypes);
    Task<Dictionary<string, IFeatureFlagDefinition>> LoadAllAsync(string microserviceScope, CancellationToken cancellationToken = default);
}
