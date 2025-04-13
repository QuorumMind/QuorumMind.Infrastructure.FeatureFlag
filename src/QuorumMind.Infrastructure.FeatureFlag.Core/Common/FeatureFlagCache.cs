using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Common;

public class FeatureFlagCache
{
    private Dictionary<string, IFeatureFlagDefinition> _flags = new();

    public void Set(Dictionary<string, IFeatureFlagDefinition> flags) =>
        Interlocked.Exchange(ref _flags, flags);

    public bool TryGet(string name, out IFeatureFlagDefinition? definition) =>
        _flags.TryGetValue(name, out definition);
}