using QuorumMind.Infrastructure.FeatureFlag.Core.Common;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Services;


public class FeatureFlagService : IFeatureFlagService
{
    private readonly FeatureFlagCache _cache;
    private readonly IFeatureFlagAuditLogger? _auditLogger;

    public FeatureFlagService(FeatureFlagCache cache, IFeatureFlagAuditLogger? auditLogger = null)
    {
        _cache = cache;
        _auditLogger = auditLogger;
    }

    public Task<bool> IsFeatureEnabledAsync(string featureName, string? userId = null, Dictionary<string, string>? attributes = null, CancellationToken cancellationToken = default)
    {
        var context = new FeatureFlagContext { FlagName = featureName, UserId = userId, Attributes = attributes };
        if (_cache.TryGet(featureName, out var def))
        {
            var enabled = def.IsEnabled(context);
            _auditLogger?.LogEvaluation(context, new FeatureFlagResult { IsEnabled = enabled, Source = def.Source });
            return Task.FromResult(enabled);
        }

        _auditLogger?.LogEvaluation(context, new FeatureFlagResult { IsEnabled = false, Source = "NotFound" });
        return Task.FromResult(false);
    }
}