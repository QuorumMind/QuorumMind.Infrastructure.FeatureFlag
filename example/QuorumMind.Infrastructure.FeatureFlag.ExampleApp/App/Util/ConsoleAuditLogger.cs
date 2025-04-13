using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Util;

public class ConsoleAuditLogger : IFeatureFlagAuditLogger
{
    public void LogEvaluation(FeatureFlagContext context, FeatureFlagResult result)
    {
        Console.WriteLine($"[Audit] Flag '{context.FlagName}' evaluated to {result.IsEnabled} (Source: {result.Source})");
    }
}