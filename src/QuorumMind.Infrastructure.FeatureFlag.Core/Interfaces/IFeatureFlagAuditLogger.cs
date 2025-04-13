using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

public interface IFeatureFlagAuditLogger
{
    void LogEvaluation(FeatureFlagContext context, FeatureFlagResult result);
}